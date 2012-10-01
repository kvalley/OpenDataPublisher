using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using System;
using System.Data;
using Odp.Data.Sql;

namespace Odp.DataServices
{

    public class sqlEntitySetHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {
        const string STARTING_KML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <kml xmlns=""http://www.opengis.net/kml/2.2"">
	            <Document>
		            <name>{0}</name>
            ";

        const string PLACEMARK_KML =
            @"		<Placemark>
				            <name><![CDATA[{1}]]></name>
				            <description><![CDATA[{2}]]></description>
            {0}
		            </Placemark>
            ";

        //more complex come directly from the snippet or geometry
        const string POINT_KML =
            @"			<Point>
					            <coordinates>{0},{1}</coordinates>
			            </Point>";

                    const string ENDING_KML =
            @"	</Document>
            </kml>
            ";

        private const string START_SERVICEDOCUMENT_TEMPLATE =
            @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
            <feed xml:base=""http://{0}/v1/{1}/{2}/"" xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
              <title type=""text"">{2}</title>
              <id>http://{0}/v1/{1}/{2}/</id>
              <updated></updated>
              <link rel=""self"" title=""{2}"" href=""{2}"" />
            ";

        private const string ENTRY_TEMPLATE =
            @"  <entry m:etag="""">
                <id>http://{0}/v1/{1}/{2}(PartitionKey='{4}',RowKey='{5}')</id>
                <title type=""text""></title>
                <updated></updated>
                <author>
                  <name />
                </author>
                <link rel=""edit"" title=""{2}"" href=""{2}(PartitionKey='{4}',RowKey='{5}')"" />
                <category term=""ODP.{1}.{2}Item"" scheme=""http://schemas.microsoft.com/ado/2007/08/dataservices/scheme"" />
                <content type=""application/xml"">
                  <m:properties>
            {3}      </m:properties>
                </content>
              </entry>
            ";

        private const string PROPERTY_TEMPLATE =
            @"        <d:{0}{1}>{2}</d:{0}>
            ";

        private const string END_SERVICEDOCUMENT_TEMPLATE =
            @"</feed>
            ";

        private const string XMLNS_ = @"xmlns:atom=""http://www.w3.org/2005/Atom"" xmlns:app=""http://www.w3.org/2007/app"" xmlns=""http://www.w3.org/2007/app""";

        public string _remainder;
        public string _ogdiAlias;
        public string _entitySet;
        private string _filter;

        private HttpContext _context;
        private string _afdPublicServiceReplacementUrl = null;
        private string _azureTableUrlToReplace = null;
        private string _entityKind;

        public string AzureTableRequestEntityUrl { get; set; }
        public string OgdiAlias { get { return _ogdiAlias; } set { _ogdiAlias = value; } }
        public string EntitySet { get; set; }

        public bool IsAvailableEndpointsRequest { get; set; }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            _entityKind = null;

            if (!this.IsHttpGet(context))
            {
                this.RespondForbidden(context);
            }
            else
            {
                if (OgdiAlias.ToLower() != "sql")
                {
                    context.Response.Write("Endpoint '" + OgdiAlias + "' does not exist.");
                    context.Response.StatusCode = 404;//internal server error (int)response.StatusCode;
                    context.Response.End();
                    return;
                }

                _context = context;

                try
                {
                    var sql = sqlServerConnection.GetDataServiceInstance();

                    _filter = _context.Request.QueryString["$filter"];
                    string top = _context.Request.QueryString["$top"];
                    string skip = _context.Request.QueryString["$skip"];
                    string columns = _context.Request.QueryString["$select"];
                    string order = _context.Request.QueryString["$orderby"];
                    int ntop = -1;
                    int nskip = 0;

                    if (!string.IsNullOrEmpty(top))
                    {
                        try
                        {
                            ntop = Convert.ToInt32(top);
                        }
                        catch
                        {
                        }
                    }

                    if (!string.IsNullOrEmpty(skip))
                    {
                        try
                        {
                            nskip = Convert.ToInt32(skip);
                        }
                        catch
                        {
                        }
                    }

                    string format = _context.Request.QueryString["format"];
                    if (!string.IsNullOrEmpty(format))
                        format = format.ToLower();

                    _context.Response.AddHeader("DataServiceVersion", "1.0;");
                    _context.Response.CacheControl = "no-cache";
                    _context.Response.AddHeader("x-ms-request-id", "-1");//TODO what ID should I use???

                    if (format == "kml")
                    {
                        RenderKml(sql/*, longlatCols, kmlCol*/);
                    }
                    else
                    {
                        //for both atom and json the starting point is OData
                        //but for atom we should feed directly to the response
                        //this means responsive GUI even with large data feeds
                        //bool odata = format != "json";

                        //these are really not next values, we're interested in values greater than, the naming remains from azure nomenclature
                        var nextPartKey = _context.Request.QueryString["NextPartitionKey"];
                        var nextRowKey = _context.Request.QueryString["NextRowKey"];

                        _entitySet = _entitySet.Replace("()", "");
                        string wholeXml = string.Format(START_SERVICEDOCUMENT_TEMPLATE, _context.Request.Url.Host, _ogdiAlias, _entitySet);

                        var md = sql.sqlGetEntitySetMetaData(_entitySet);
                        bool isRowId = !string.IsNullOrEmpty(md.RowId);
                        var sql2 = sqlServerConnection.GetDataServiceInstance();
                        DataTable reader = sql2.GetReader(md, columns, _filter, ntop, nextRowKey, nskip, order);

                        //for easy string comparison ...
                        md.Longitude = md.Longitude.ToLower();
                        md.Latitude = md.Latitude.ToLower();
                        md.KmlSnippet = md.KmlSnippet.ToLower();

                        //this is not OData friendly, but...
                        //when columns are specified as containing KML data we don't show them because
                        //1. kml is not OData, it must be fed via format=kml
                        //2. some kml snippets could be very large
                        //string[] cols = (longlatCols ?? "").ToLower().Split(',');//we 
                        //string col = (kmlCol ?? "").ToLower();

                        var fmt = new NumberFormatInfo() { NumberDecimalSeparator = "." };
                        int read = 0;
                        string partitionKey = "";//unused
                        string rowKey = "";//read if defined in the metadata - we're interested in the last one, but we read all of them
                        StringBuilder sb = new StringBuilder(wholeXml);
                        
                        foreach(DataRow row in reader.Rows)
                        {
                            read++;
                            string properties = "";
                            properties += string.Format(PROPERTY_TEMPLATE, "PartitionKey", string.Empty, "");
                            properties += string.Format(PROPERTY_TEMPLATE, "RowKey", string.Empty, "");
                            for (int i = 0; i < reader.Columns.Count; i++)
                            {
                                string name = reader.Columns[i].ColumnName;
                                string lName = name.ToLower();

                                if (lName == md.KmlSnippet)
                                    continue;//exclude

                                Type type = row[i].GetType();
                                string val = row[i].ToString();
                                string key = string.Empty;
                                string valType = string.Empty;

                                if (!Convert.IsDBNull(row[i]))
                                {
                                    if (type == typeof(double))
                                        val = Convert.ToDouble(row[i]).ToString(fmt);
                                    else if (type == typeof(decimal))
                                        val = Convert.ToDecimal(row[i]).ToString(fmt);
                                    else if (type == typeof(DateTime))
                                    {
                                        val = Convert.ToDateTime(row[i]).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                                        valType = " " + "m:type=\"Edm.DateTime\"";
                                    }
                                    else
                                        val = row[i].ToString();

                                    key = val;

                                    if (type == typeof(string))
                                    {
                                        val = "<![CDATA[" + val + "]]>";
                                    }
                                }

                                properties += string.Format(PROPERTY_TEMPLATE, name, valType, val);

                                if (isRowId)
                                {
                                    if (name == md.RowId)
                                        rowKey = key;
                                }
                            }

                            string entry = string.Format(ENTRY_TEMPLATE, context.Request.Url.Host, _ogdiAlias, _entitySet, properties, partitionKey, rowKey);
                            sb.Append(entry);
                        }
                        wholeXml = sb.ToString();
                        sb.Clear();
                        sb = null;

                        wholeXml += END_SERVICEDOCUMENT_TEMPLATE;

                        if (read == ntop) //there is still data left to be read, continue paging
                        {
                            //sl-king
                            //Azure seems to use two keys to identify a row - PartitionKey and RowKey (it doesn't really matter here)
                            //the Interactive SDK uses both to support data paging
                            //with SQL server we use only RowKey and for the PartitionKey just use "dummy" value just to enable paging

                            //string continuationNextPartitionKey = response.Headers["x-ms-continuation-NextPartitionKey"];
                            //if (continuationNextPartitionKey != null)
                            {
                                _context.Response.AddHeader("x-ms-continuation-NextPartitionKey", "dummy");//continuationNextPartitionKey
                            }

                            //string continuationNextRowKey = response.Headers["x-ms-continuation-NextRowKey"];
                            //if (continuationNextRowKey != null)
                            {
                                _context.Response.AddHeader("x-ms-continuation-NextRowKey", rowKey);//continuationNextRowKey
                            }
                        }

                        if (format == "json")
                        {
                            _context.Response.ContentType = "application/json";
                            var xml = XElement.Parse(wholeXml);
                            this.RenderJson(xml);
                        }
                        else
                        {
                            _context.Response.ContentType = "application/atom+xml;charset=utf-8";
                            _context.Response.Write(wholeXml);
                        }
                    }

                    if (true)//TODO implement
                    {
                    }
                    else//TODO implement kml and json
                    {
                     
                    }
                }
                catch (Exception ex)
                {
                    Odp.Data.ErrorLog.WriteError(ex.Message);
                }
            }
        }

        private string LoadEntityKind(HttpContext context, string entitySet)
        {
            throw new Exception("Overlooked function LoadEntityKind");
            return "";
        }

        #endregion

        private void RenderKml(sqlServerConnection sql/*, string longlatCols_, string kmlCol*/)
        {
            bool kmlFromLonglat = false;
            bool kmlFromSnippet = false;
            bool kmlName = false;
            bool kmlDesc = false;
            string longitude = "";//column
            string latitude = "";//column
            string snippet = "";//column
            string name = "";//column
            string description = "";//column

            //get KML metadata
            var reader = sql.GetReader("select * from odpdataservicemetadata where EntitysetName='" + _entitySet + "';");
            if (reader.Rows.Count > 0)//just read one
            {
                //TODO can we recycle this, see gskhasdfsfdsagf
                //exclude this from the OData feed
                var lon = reader.Rows[0]["Longitude"];
                var lat = reader.Rows[0]["Latitude"];
                var kml_ = reader.Rows[0]["KmlSnippet"];
                var name_ = reader.Rows[0]["KmlName"];
                var desc = reader.Rows[0]["KmlDescription"];

                kmlFromLonglat = lon.GetType() != typeof(System.DBNull) && lat.GetType() != typeof(System.DBNull);
                kmlFromSnippet = !kmlFromLonglat && kml_.GetType() != typeof(System.DBNull);
                kmlName = name_.GetType() != typeof(System.DBNull);
                kmlDesc = desc.GetType() != typeof(System.DBNull);

                if (kmlFromLonglat)
                {
                    longitude = lon.ToString();
                    latitude = lat.ToString();
                }

                if (kmlFromSnippet)
                    snippet = kml_.ToString();

                if (kmlName)
                    name = name_.ToString();

                if (kmlDesc)
                    description = desc.ToString();
            }

            string columns;//to select from the table

            if (kmlFromLonglat || kmlFromSnippet)
            {
                if (kmlFromLonglat)
                    columns = longitude + ", " + latitude;
                else
                    columns = snippet;
                if (kmlName)
                    columns += ", " + name;
                if (kmlDesc)
                    columns += ", " + description;
            }
            else//geometry property
            {
                _context.Response.StatusCode = 404;//not found
                _context.Response.End();
                return;
            }

            _context.Response.ContentType = "application/vnd.google-earth.kml+xml";
            _context.Response.Write(string.Format(STARTING_KML, _entitySet));

            string table = sql.GetTableName(_ogdiAlias, _entitySet);
            string where = sqlServerConnection.WcfToSqlFilter(_filter);

            string sqltest = "SELECT " + columns + " FROM " + table + (string.IsNullOrEmpty(where) ? "" : (" WHERE " + where)) + ";";

            reader = sql.GetReader(
                "SELECT " + columns + " FROM " + table +
                (string.IsNullOrEmpty(where) ? "" : (" WHERE " + where)) + ";");

            var format = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            foreach(DataRow row in reader.Rows)
            {
                if (!Convert.IsDBNull(row[0]) && !Convert.IsDBNull(row[1]))
                {
                    string placemark = "";

                    if (kmlFromLonglat || kmlFromSnippet)
                    {
                        var nameVal = kmlName ? row[name].ToString() : "";
                        var descVal = kmlName ? row[description].ToString() : "";
                        string geom = "";
                        if (kmlFromLonglat)
                            geom = string.Format(POINT_KML, Convert.ToDouble(row[0]).ToString(format), Convert.ToDouble(row[1]).ToString(format));//TODO include Z if needed
                        else
                            if (kmlFromSnippet)
                            {
                                geom = row[snippet].ToString();
                                if (string.IsNullOrEmpty(geom))
                                    continue;
                            }
                        placemark = string.Format(PLACEMARK_KML, geom, nameVal, descVal);
                    }
                    else//from geometry otherwise we wouldn't be here
                    {
                        placemark = "kmlfrom geometry not implemented\n";
                    }

                    _context.Response.Write(placemark);
                }
            }
            _context.Response.Write(ENDING_KML);
        }

        private void RenderJson(XElement feed)
        {
            _context.Response.ContentType = "application/json";
            XName kmlSnippetElementString = _nsd + "kmlsnippet";

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("d");
                jsonWriter.WriteStartArray();

                IEnumerable<XElement> propertiesElements = GetPropertiesElements(feed);

                foreach (var propertiesElement in propertiesElements)
                {
                    jsonWriter.WriteStartObject();

                    propertiesElement.Elements(kmlSnippetElementString).Remove();

                    foreach (var element in propertiesElement.Elements())
                    {
                        jsonWriter.WritePropertyName(element.Name.LocalName);
                        jsonWriter.WriteValue(element.Value);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
                jsonWriter.WriteEndObject();
            }

            var callbackFunctionName = _context.Request["callback"];

            if (callbackFunctionName != null)
            {
                _context.Response.Write(callbackFunctionName);
                _context.Response.Write("(");
                _context.Response.Write(sb.ToString());
                _context.Response.Write(");");
            }
            else
            {
                _context.Response.Write(sb.ToString());
            }
        }

        private void RenderAtomPub(XElement feed)
        {
            // Update Azure Table Storage url for //feed/id
            string idValue = feed.Element(_idXName).Value;
            string baseValue = feed.Attribute(XNamespace.Xml + "base").Value;

            feed.Attribute(XNamespace.Xml + "base").Value = this.ReplaceAzureUrlInString(baseValue);

            feed.Element(_idXName).Value = this.ReplaceAzureUrlInString(idValue);

            // The xml payload coming back has a <kmlsnippet> property.  We want to
            // hide that from the consumer of our service by removing it.
            // NOTE: We only use kmlsnippet when returning KML.

            // Iterate through all the entries to update 
            // Azure Table Storage url for //feed/entry/id
            // and remove kmlsnippet element from all instances of
            // //feed/entry/content/properties

            IEnumerable<XElement> entries = feed.Elements(_entryXName);

            bool isSingleEntry = true;

            foreach (var entry in entries)
            {
                isSingleEntry = false;

                idValue = entry.Element(_idXName).Value;
                entry.Element(_idXName).Value = this.ReplaceAzureUrlInString(idValue);

                ReplaceAzureNamespaceInCategoryTermValue(entry);

                var properties = entry.Elements(_contentXName).Elements(_propertiesXName);

                if (!this.IsAvailableEndpointsRequest)
                {
                    properties.Elements(_kmlSnippetXName).Remove();
                }
                else
                {
                    properties.Elements(_storageAccountNameXName).Remove();
                    properties.Elements(_storageAccountKeyXName).Remove();
                }
            }

            if (isSingleEntry)
            {
                ReplaceAzureNamespaceInCategoryTermValue(feed);
                var properties = feed.Elements(_contentXName).Elements(_propertiesXName);
                properties.Elements(_kmlSnippetXName).Remove();
            }

            _context.Response.ContentType = "application/atom+xml;charset=utf-8";
            _context.Response.Write(feed.ToString());
        }

        private void ReplaceAzureNamespaceInCategoryTermValue(XElement entry)
        {
            // use the simple approach of representing "entitykind" as
            // "entityset" value plus the text "Item."  A decision was made to do
            // this at the service level for now so that we wouldn't have to deal 
            // with changing the data import code and the existing values in the 
            // EntityMetadata table.

            var term = entry.Element(_categoryXName).Attribute("term");

            //TODO: apply real fix. OgdiAlias is null for AvailableEndpoints
            if (this.OgdiAlias != null)
            {
                if (_entityKind == null)
                {
                    var termValue = term.Value;
                    var dotLocation = termValue.ToString().IndexOf(".");
                    var entitySet = termValue.Substring(dotLocation + 1);
                    _entityKind = LoadEntityKind(_context, entitySet);
                }
                term.Value = string.Format(_termNamespaceString, this.OgdiAlias.ToLower(), _entityKind);
            }
        }

        private string ReplaceAzureUrlInString(string xmlString)
        {
            // The xml payload returned from Table Storage data service has urls
            // that point back to Table Storage.  We need to replace the urls with the
            // proper urls for our public service.
            return xmlString.Replace(_azureTableUrlToReplace, _afdPublicServiceReplacementUrl);
        }

        private static IEnumerable<XElement> GetPropertiesElements(XElement feed)
        {
            return feed.Elements(_entryXName).Elements(_contentXName).Elements(_propertiesXName);
        }
    }
}
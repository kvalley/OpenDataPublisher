using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using System;
using Odp.Data.Sql;

namespace Odp.DataServices
{

    public class sqlAppAvailableEndpointsHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {

        private const string START_SERVICEDOCUMENT_TEMPLATE =
@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
<feed xml:base='{0}' xmlns:d='http://schemas.microsoft.com/ado/2007/08/dataservices' xmlns:m='http://schemas.microsoft.com/ado/2007/08/dataservices/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>AvailableEndpoints</title>
  <id>{0}</id>
  <updated></updated>
  <link rel='self' title='AvailableEndpoints' href='AvailableEndpoints' />
";

        private const string ENTRY_TEMPLATE =
@"
	<entry m:etag=''>
		<id>{0}</id>
		<title type='text'></title>
		<updated></updated>
		<author>
			<name />
		</author>
		<category term='ogdiconfiguration.AvailableEndpoints' scheme='http://schemas.microsoft.com/ado/2007/08/dataservices/scheme' />
		<content type='application/xml'>
			<m:properties>
				<d:PartitionKey>{1}</d:PartitionKey>
				<d:RowKey></d:RowKey>
				<d:Timestamp m:type='Edm.DateTime'></d:Timestamp>
				<d:alias>{1}</d:alias>
				<d:description>{2}</d:description>
				<d:disclaimer>{3}</d:disclaimer>
			</m:properties>
		</content>
	</entry>
";

        private const string END_SERVICEDOCUMENT_TEMPLATE =
@"	</feed>
";

        private const string XMLNS_ = @"xmlns:atom='http://www.w3.org/2005/Atom' xmlns:app='http://www.w3.org/2007/app' xmlns='http://www.w3.org/2007/app'";

        private HttpContext _context;
        //private string _afdPublicServiceReplacementUrl;
        //private string _azureTableUrlToReplace;
        //private string _entityKind;

        //public string AzureTableRequestEntityUrl { get; set; }
        //public string OgdiAlias { get; set; }
        //public string EntitySet { get; set; }

        //public bool IsAvailableEndpointsRequest { get; set; }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        //private string LoadEntityKind(HttpContext context, string entitySet)
        //{
        //  WebRequest request;

        //  var accountName = AppSettings.EnabledStorageAccounts[this.OgdiAlias].storageaccountname;
        //  var accountKey = AppSettings.EnabledStorageAccounts[this.OgdiAlias].storageaccountkey;

        //  var requestUrl = AppSettings.TableStorageBaseUrl + "TableMetadata";
        //  request = this.CreateTableStorageSignedRequest(context, accountName, accountKey, requestUrl, false, true);

        //  try
        //  {
        //      var response = request.GetResponse();
        //      var responseStream = response.GetResponseStream();

        //      var feed = XElement.Load(XmlReader.Create(responseStream));

        //      var propertiesElements = feed.Elements(_nsAtom + "entry").Elements(_nsAtom + "content").Elements(_nsm + "properties");

        //      foreach (var e in propertiesElements)
        //      {
        //          if(entitySet == e.Element(_nsd + "entityset").Value)
        //              return e.Element(_nsd + "entitykind").Value;
        //      }

        //  }
        //  catch (WebException ex)
        //  {
        //      var response = ex.Response as HttpWebResponse;
        //      context.Response.StatusCode = (int)response.StatusCode;
        //      context.Response.End();
        //  }
        //  return null;
        //}

        public void ProcessRequest(HttpContext context)
        {
            //_entityKind = null;

            if (!this.IsHttpGet(context))
            {
                this.RespondForbidden(context);
            }
            else
            {
                _context = context;
                //WebRequest request;
                //string accountName;
                //string accountKey;

                //if (!this.IsAvailableEndpointsRequest)
                //{
                //    // See AvailableEndpoint.cs for explanation why properties are all lowercase
                //    accountName = AppSettings.EnabledStorageAccounts[this.OgdiAlias].storageaccountname;
                //    accountKey = AppSettings.EnabledStorageAccounts[this.OgdiAlias].storageaccountkey;

                //}
                //else
                //{
                //    accountName = AppSettings.OgdiConfigTableStorageAccountName;
                //    accountKey =  AppSettings.OgdiConfigTableStorageAccountKey;
                //}

                //request = this.CreateTableStorageSignedRequest(context, accountName, accountKey,
                //                                               this.AzureTableRequestEntityUrl,
                //                                               this.IsAvailableEndpointsRequest);

                //Action<string,string,string> incView = AnalyticsRepository.RegisterView;
                //incView.BeginInvoke(String.Format("{0}||{1}", this.OgdiAlias, this.EntitySet), 
                //    context.Request.RawUrl,
                //    context.Request.UserHostName,
                //    null, null);

                try
                {
                    //var response = request.GetResponse();
                    //var responseStream = response.GetResponseStream();

                    //var feed = XElement.Load(XmlReader.Create(responseStream));

                    //_context.Response.Headers["DataServiceVersion"] = "1.0;";
                    _context.Response.AddHeader("DataServiceVersion", "1.0;");
                    _context.Response.CacheControl = "no-cache";
                    //_context.Response.AddHeader("x-ms-request-id", response.Headers["x-ms-request-id"]);
                    _context.Response.AddHeader("x-ms-request-id", "-1");//TODO what ID should we set?

                    var xmlBase = "http://" + _context.Request.Url.Host + _context.Request.Url.AbsolutePath;
                    string wholeXml = string.Format(START_SERVICEDOCUMENT_TEMPLATE, xmlBase);

                    var list = sqlAppSettings.RefreshAvailableEndpoints();
                    foreach (var item in list)
                    {
                        wholeXml += string.Format(ENTRY_TEMPLATE, xmlBase, item.Value.alias, item.Value.description, item.Value.disclaimer);
                    }

                    wholeXml += END_SERVICEDOCUMENT_TEMPLATE;

                    //var feed = XElement.Load(XmlReader.Create(wholeXml));

                    _context.Response.ContentType = "application/atom+xml;charset=utf-8";
                    _context.Response.Write(wholeXml);

                    //string continuationNextPartitionKey = response.Headers["x-ms-continuation-NextPartitionKey"];

                    //if (continuationNextPartitionKey != null)
                    //{
                    //    _context.Response.AddHeader("x-ms-continuation-NextPartitionKey", continuationNextPartitionKey);
                    //}

                    //string continuationNextRowKey = response.Headers["x-ms-continuation-NextRowKey"];

                    //if (continuationNextRowKey != null)
                    //{
                    //    _context.Response.AddHeader("x-ms-continuation-NextRowKey", continuationNextRowKey);
                    //}

                    //string format = _context.Request.QueryString["format"];

                    //this.SetupReplacementUrls();

                    //switch (format)
                    //{
                    //    case "kml":
                    //        this.RenderKml(feed);
                    //        break;
                    //    case "json":
                    //        this.RenderJson(feed);
                    //        break;
                    //    default:
                    //        // If "format" is not kml or json, then assume AtomPub
                    //        this.RenderAtomPub(feed);
                    //        break;
                    //}
                }
                catch (Exception ex)
                {
                    Odp.Data.ErrorLog.WriteError(ex.Message);
                }
            }
        }

        #endregion

        //private void RenderKml(XElement feed)
        //{
        //    const string STARTING_KML = "<kml xmlns=\"http://www.opengis.net/kml/2.2\"><Document><name></name>";
        //    const string ENDING_KML = "</Document></kml>";

        //    _context.Response.ContentType = "application/vnd.google-earth.kml+xml";

        //    _context.Response.Write(STARTING_KML);

        //    var propertiesElements = GetPropertiesElements(feed);

        //    foreach (var propertiesElement in propertiesElements)
        //    {
        //        var kmlSnippet = propertiesElement.Element(_nsd + "kmlsnippet");

        //        if (kmlSnippet != null)
        //        {
        //            // If the kmlsnippet size is <= 64K, then we just store
        //            // it in the <kmlsnippet/> element.  However, due to the
        //            // 64K string storage limitations in Azure Tables,
        //            // we store larger KML snippets in a Azure Blob.
        //            // In this case the <kmlsnippet/> element contains:
        //            //      <KmlSnippetReference><Container>zipcodes</Container><Blob>33a8d702-c21b-4b09-8cdb-a09cef2e3115</Blob></KmlSnippetReference>
        //            // We need to parse this string into an XElement and then
        //            // go get the kml snippet out of the blob.
        //            // From a perf perspective, this is not ideal.  
        //            // However, "it is what it is."

        //            var kmlSnippetValue = kmlSnippet.Value;

        //            if (kmlSnippetValue.Contains("KmlSnippetReference"))
        //            {
        //                var blobId = XElement.Parse(kmlSnippetValue).Element("Blob").Value;
        //                var request = this.CreateBlobStorageSignedRequest(blobId, this.OgdiAlias, this.EntitySet);
        //                var response = request.GetResponse();
        //                var strReader = new StreamReader(response.GetResponseStream());
        //                var kmlSnippetString = strReader.ReadToEnd();

        //                _context.Response.Write(kmlSnippetString);
        //            }
        //            else
        //            {
        //                _context.Response.Write(kmlSnippetValue);
        //            }
        //        }
        //    }

        //    _context.Response.Write(ENDING_KML);
        //}

        //private void RenderJson(XElement feed)
        //{
        //    _context.Response.ContentType = "application/json";
        //    XName kmlSnippetElementString = _nsd + "kmlsnippet";

        //    StringBuilder sb = new StringBuilder();
        //    StringWriter sw = new StringWriter(sb);

        //    using (JsonWriter jsonWriter = new JsonTextWriter(sw))
        //    {
        //        jsonWriter.WriteStartObject();
        //        jsonWriter.WritePropertyName("d");
        //        jsonWriter.WriteStartArray();

        //        IEnumerable<XElement> propertiesElements = GetPropertiesElements(feed);

        //        foreach (var propertiesElement in propertiesElements)
        //        {
        //            jsonWriter.WriteStartObject();

        //            propertiesElement.Elements(kmlSnippetElementString).Remove();

        //            foreach (var element in propertiesElement.Elements())
        //            {
        //                jsonWriter.WritePropertyName(element.Name.LocalName);
        //                jsonWriter.WriteValue(element.Value);
        //            }

        //            jsonWriter.WriteEndObject();
        //        }

        //        jsonWriter.WriteEndArray();
        //        jsonWriter.WriteEndObject();
        //    }

        //    var callbackFunctionName = _context.Request["callback"];

        //    if (callbackFunctionName != null)
        //    {
        //        _context.Response.Write(callbackFunctionName);
        //        _context.Response.Write("(");
        //        _context.Response.Write(sb.ToString());
        //        _context.Response.Write(");");
        //    }
        //    else
        //    {
        //        _context.Response.Write(sb.ToString());
        //    }
        //}

        //private void RenderAtomPub(XElement feed)
        //{   
        //    // Update Azure Table Storage url for //feed/id
        //    string idValue = feed.Element(_idXName).Value;
        //    string baseValue = feed.Attribute(XNamespace.Xml + "base").Value;

        //    feed.Attribute(XNamespace.Xml + "base").Value = this.ReplaceAzureUrlInString(baseValue);

        //    feed.Element(_idXName).Value = this.ReplaceAzureUrlInString(idValue);

        //    // The xml payload coming back has a <kmlsnippet> property.  We want to
        //    // hide that from the consumer of our service by removing it.
        //    // NOTE: We only use kmlsnippet when returning KML.

        //    // Iterate through all the entries to update 
        //    // Azure Table Storage url for //feed/entry/id
        //    // and remove kmlsnippet element from all instances of
        //    // //feed/entry/content/properties

        //    IEnumerable<XElement> entries = feed.Elements(_entryXName);

        //    bool isSingleEntry = true;

        //    foreach (var entry in entries)
        //    {
        //        isSingleEntry = false;

        //        idValue = entry.Element(_idXName).Value;
        //        entry.Element(_idXName).Value = this.ReplaceAzureUrlInString(idValue);

        //        //ReplaceAzureNamespaceInCategoryTermValue(entry);

        //        var properties = entry.Elements(_contentXName).Elements(_propertiesXName);

        //        if (!this.IsAvailableEndpointsRequest)
        //        {
        //            properties.Elements(_kmlSnippetXName).Remove();
        //        }
        //        else 
        //        {
        //            properties.Elements(_storageAccountNameXName).Remove();
        //            properties.Elements(_storageAccountKeyXName).Remove();
        //        }
        //    }

        //    if (isSingleEntry)
        //    {
        //        //ReplaceAzureNamespaceInCategoryTermValue(feed);
        //        var properties = feed.Elements(_contentXName).Elements(_propertiesXName);
        //        properties.Elements(_kmlSnippetXName).Remove();
        //    }

        //    _context.Response.ContentType = "application/atom+xml;charset=utf-8";
        //    _context.Response.Write(feed.ToString());
        //}

        //private void ReplaceAzureNamespaceInCategoryTermValue(XElement entry)
        //{
        //    // use the simple approach of representing "entitykind" as
        //    // "entityset" value plus the text "Item."  A decision was made to do
        //    // this at the service level for now so that we wouldn't have to deal 
        //    // with changing the data import code and the existing values in the 
        //    // EntityMetadata table.

        //    var term = entry.Element(_categoryXName).Attribute("term");

        //    //TODO: apply real fix. OgdiAlias is null for AvailableEndpoints
        //    if (this.OgdiAlias != null)
        //    {
        //        if (_entityKind == null)
        //        {
        //            var termValue = term.Value;
        //            var dotLocation = termValue.ToString().IndexOf(".");
        //            var entitySet = termValue.Substring(dotLocation + 1);
        //            _entityKind = LoadEntityKind(_context, entitySet);
        //        }
        //        term.Value = string.Format(_termNamespaceString, this.OgdiAlias.ToLower(), _entityKind);
        //    }
        //}

        //private void SetupReplacementUrls()
        //{
        //    // The xml payload returned from Table Storage data service has urls
        //    // that point back to Table Storage.  We need to replace the urls with the
        //    // proper urls for our public service.
        //    StringBuilder sb = new StringBuilder(_context.Request.Url.Scheme); 
        //    sb.Append("://"); 
        //    sb.Append(_context.Request.Url.Host); 
        //    sb.Append("/v1/");            

        //    if (!this.IsAvailableEndpointsRequest)
        //    {
        //        sb.Append(this.OgdiAlias);
        //        sb.Append("/");

        //        _azureTableUrlToReplace =
        //            string.Format(AppSettings.TableStorageBaseUrl,
        //                                        AppSettings.EnabledStorageAccounts[this.OgdiAlias].storageaccountname);
        //    }
        //    else
        //    {
        //        _azureTableUrlToReplace =
        //            string.Format(AppSettings.TableStorageBaseUrl,
        //                                        AppSettings.OgdiConfigTableStorageAccountName);
        //    }

        //    _afdPublicServiceReplacementUrl = sb.ToString();
        //}

        //private string ReplaceAzureUrlInString(string xmlString)
        //{
        //    // The xml payload returned from Table Storage data service has urls
        //    // that point back to Table Storage.  We need to replace the urls with the
        //    // proper urls for our public service.
        //    return xmlString.Replace(_azureTableUrlToReplace, _afdPublicServiceReplacementUrl);
        //}

        //private static IEnumerable<XElement> GetPropertiesElements(XElement feed)
        //{
        //    return feed.Elements(_entryXName).Elements(_contentXName).Elements(_propertiesXName);
        //}
    }
}
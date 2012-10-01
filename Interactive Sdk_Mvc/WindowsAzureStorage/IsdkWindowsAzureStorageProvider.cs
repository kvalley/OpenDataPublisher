using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Odp.InteractiveSdk.Mvc;
using System.Xml.Linq;
using System.Configuration;
using System.Net;
using System.Xml;
using System.IO;
using System.Web;
using System.Data;
using System.Collections;

namespace Odp.InteractiveSdk.Mvc
{
    public class IsdkWindowsAzureStorageProvider : IsdkStorageProviderInterface
    {
        #region Properties

        public string ServiceUri { get; set; }
        public string PathDTD { get; set; }

        #endregion

        #region Constructors

        public IsdkWindowsAzureStorageProvider(string serviceUri, string pathDTD)
        {
            ServiceUri = serviceUri;
            PathDTD = pathDTD;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to create serviceUri through parameters passed.
        /// </summary>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter query</param>
        /// <param name="pageSize">Number of rows to be fetched</param>
        /// <param name="nextPartitionKey">PartionKey to fetch next partion data</param>
        /// <param name="nextRowKey">RowKey to fetch row information</param>
        /// <returns>Returns constructed serviceUri</returns>
        private Uri LoadServiceUri(string uri, string container, string tableName,
                                    string filter, int pageSize,
                                    string nextPartitionKey, string nextRowKey,
                                                                        int skip)
        {
            var serviceUriBuilder = new StringBuilder();

            //serviceUriBuilder.Append(ConfigurationManager.AppSettings["serviceUri"]);
            //serviceUriBuilder.Append(RoleEnvironment.GetConfigurationSettingValue("serviceUri"));
            serviceUriBuilder.Append(string.IsNullOrEmpty(uri) ? ServiceUri : uri);
            if (container != null && container != "")
            {
                serviceUriBuilder.Append(container);
                serviceUriBuilder.Append("/");
            }

            if (tableName != null && tableName != "")
            {
                serviceUriBuilder.Append(tableName);
                serviceUriBuilder.Append("/");
            }

            string delimiter = "?";

            if (pageSize > 0)
            {
                serviceUriBuilder.Append(delimiter + "$top=");
                serviceUriBuilder.Append(pageSize);
                delimiter = "&";
            }

            if (skip > 0)
            {
                serviceUriBuilder.Append(delimiter + "$skip=");
                serviceUriBuilder.Append(skip);
                delimiter = "&";
            }

            if (!String.IsNullOrEmpty(filter))
            {
                serviceUriBuilder.Append(delimiter + "$filter=");
                serviceUriBuilder.Append(filter);
                delimiter = "&";
            }

            if (!String.IsNullOrEmpty(nextPartitionKey) && !String.IsNullOrEmpty(nextRowKey))
            {
                serviceUriBuilder.Append(delimiter + "NextPartitionKey=");
                serviceUriBuilder.Append(nextPartitionKey);
                delimiter = "&";
                serviceUriBuilder.Append("NextRowKey=");
                serviceUriBuilder.Append(nextRowKey);
            }

            return new Uri(serviceUriBuilder.ToString());
        }

        /// <summary>
        /// Strip namespaces from a hierarchy of XML elements.
        /// </summary>
        /// <param name="root">XML element.</param>
        /// <returns>Same as <paramref>root</paramref> parameter.</returns>
        /// Note:-This Code has been referred from previous Asp.net project.
        private static XElement StripNamespaces(XElement root)
        {
            // found this code at http://social.msdn.microsoft.com/Forums/en-US/linqprojectgeneral/thread/bed57335-827a-4731-b6da-a7636ac29f21/
            foreach (XElement e in root.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                {
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                }
                if (e.Attributes().Where(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None).Any())
                {
                    e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
                }
            }

            return root;
        }

        /// <summary>
        /// Function to get all the Headers for a particular Entity Set
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void AddMetadataToXElement(string realUri, string containerAlias, XElement data)
        {
            var propertiesElements = data.Elements("properties");
            int nResults = propertiesElements.Count();

            if (nResults > 0)
            {
                var tableName = data.Attribute("tableName").Value;

                //sl-king
                //TODO make this a reusable code (find all sfdgsdfgsdfgd)
                //EntitySet es = EntitySetRepository.GetEntitySet(container, entitySet);
                //since this is OData address looks like this .../service/category/entityset
                //metadata is at address .../service/category/$metadata
                string metadataUri = realUri;

                //first get rid of ?option1=...&option2=....
                var uriParts = metadataUri.Split('?');
                metadataUri = uriParts[0];
                if (metadataUri[metadataUri.Length - 1] != '/')//if it's not / terminated, let's terminate it
                    metadataUri = metadataUri + "/";

                uriParts = metadataUri.Split('/');
                metadataUri = uriParts[0];
                for (int i = 1; i < uriParts.Length - 2; i++)
                {
                    metadataUri += "/" + uriParts[i];
                }
                metadataUri += "/$metadata";



                var filter = "entityset eq '" + tableName + "'";
                var enityMetaData = GetMetadata(metadataUri, containerAlias, tableName);//, filter);

                //remove the OGDI inherited virtual columns
                var et = enityMetaData.Element("EntityType");
                et.Element("Key").Remove();
                var props = et.Elements("Property");
                var toberemoved = new List<XElement>();//can't remove them within the loop
                foreach (var prop in props)
                    if (prop.Attribute("Name").Value == "PartitionKey" || prop.Attribute("Name").Value == "RowKey")
                        toberemoved.Add(prop);
                foreach (var prop in toberemoved)
                    prop.Remove();

                data.FirstNode.AddBeforeSelf(enityMetaData.FirstNode);
            }
        }

        #endregion

        #region Storage Overrided Methods

        //sl-king
        //uri is for overload, since every table has its own OData uri

        /// <summary>
        /// Get an XML element containing data from the specified table + container combination, 
        /// filtering according to the filter criteria specified by the caller.
        /// </summary>
        /// <param name="container">Alias of the container, pass null for all records.</param>
        /// <param name="tableName">EntitySet\Table name, pass null for all records.</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <param name="pageSize">Number of rows to be fetched</param>
        /// <param name="nextPartitionKey">PartionKey to fetch next partion data</param>
        /// <param name="nextRowKey">RowKey to fetch row information</param>
        /// <returns>An XML element containing the results of the query.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="WebException"></exception>
        /// <exception cref="Exception"></exception>
        public override XElement GetData(string uri, string container, string tableName,
                                        string filter, int pageSize,
                                        string nextPartitionKey, string nextRowKey,
                                                                                int skip)
        {
            XElement xmlData = null;
            Uri temp;

            try
            {
                Uri serviceUri = string.IsNullOrEmpty(uri) ?
                                  LoadServiceUri(uri, container, tableName, filter, pageSize, nextPartitionKey, nextRowKey, skip) :
                                  LoadServiceUri(uri, "", "", filter, pageSize, nextPartitionKey, nextRowKey, skip);
                temp = serviceUri;

                // Store the partitionkey and rowkey as prevpartitionkey and prevrowkey before getting new set of data.
                string currentPartitionKeyStr = string.Empty;
                string currentRowKeyStr = string.Empty;
                if (!string.IsNullOrEmpty(nextPartitionKey))
                {
                    currentPartitionKeyStr = nextPartitionKey;
                }

                if (!string.IsNullOrEmpty(nextRowKey))
                {
                    currentRowKeyStr = nextRowKey;
                }

                string nextPartitionKeyStr = string.Empty;
                string nextRowKeyStr = string.Empty;

                var webRequest = HttpWebRequest.Create(serviceUri);
                var response = webRequest.GetResponse();
                var responseStream = response.GetResponseStream();

                if (response.Headers[AzureResources.continuation_nextPartionKey] != null)
                    nextPartitionKeyStr = response.Headers[AzureResources.continuation_nextPartionKey];

                if (response.Headers[AzureResources.continuation_nextRowKey] != null)
                    nextRowKeyStr = response.Headers[AzureResources.continuation_nextRowKey];

                var feed = XElement.Load(XmlReader.Create(responseStream));

                //this is ugly, but OGDI feeds are different for different queries
                IEnumerable<XElement> propertiesElements;
                if (string.IsNullOrEmpty(container) && tableName == "AvailableEndpoints")
                    propertiesElements = feed.Elements(XNamespace.Get(AzureResources.nsAtom) + "entry").Elements(XNamespace.Get(AzureResources.nsAtom) + "content").Elements(XNamespace.Get(AzureResources.nsMetadata) + "properties");
                else
                    if (!string.IsNullOrEmpty(container) && tableName == "$metadata")//v1/sql/$metadata
                    {
                        XNamespace edmx = "http://schemas.microsoft.com/ado/2007/06/edmx";
                        XNamespace edm = "http://schemas.microsoft.com/ado/2007/05/edm";
                        var ds = feed.Element(edmx + "DataServices");
                        var schema = ds.Element(edm + "Schema");
                        propertiesElements = schema.Elements(edm + "EntityType");
                        propertiesElements = propertiesElements.Concat(schema.Elements(edm + "EntityContainer"));//we also need this, EntityType and EntitySet may not be named identically + there may be namespaces involved
                    }
                    else
                        //if (!string.IsNullOrEmpty(container) && string.IsNullOrEmpty(tableName))//v1/sql/
                        if (!string.IsNullOrEmpty(container) && (container == "EntitySets" || container == "Metadata"))//v1/sql/
                        {
                            XNamespace app = "http://www.w3.org/2007/app";
                            var workspace = feed.Element(app + "workspace");
                            propertiesElements = workspace.Elements(app + "collection");
                        }
                        else
                            if (!string.IsNullOrEmpty(container) && !string.IsNullOrEmpty(tableName))//v1/sql/entityset
                            {
                                XNamespace atom = "http://www.w3.org/2005/Atom";
                                propertiesElements = feed.Elements(atom + "entry");
                            }
                            else
                                throw new Exception("Implement for other OGDI queries");

                // Remove PartitionKey, RowKey, and Timestamp because we don't want users to focus on these.
                // They are required by Azure Table storage, but will most likely go away
                // when we move to SDS.
                propertiesElements.Elements(XNamespace.Get(AzureResources.nsDataServices) + "PartitionKey").Remove();
                propertiesElements.Elements(XNamespace.Get(AzureResources.nsDataServices) + "RowKey").Remove();
                propertiesElements.Elements(XNamespace.Get(AzureResources.nsDataServices) + "Timestamp").Remove();

                // XmlDataSource doesn't support namespaces well
                // http://www.hanselman.com/blog/PermaLink,guid,8147b263-24fc-498d-83d1-546f4dde3fc3.aspx
                // Therefore, we will return XML that doesn't have any
                XElement root = new XElement("Root", propertiesElements);
                root.Add(new XAttribute("tableName", tableName));
                root.Add(new XAttribute("currentPartitionKey", currentPartitionKeyStr));
                root.Add(new XAttribute("currentRowKey", currentRowKeyStr));
                root.Add(new XAttribute("nextPartitionKey", nextPartitionKeyStr));
                root.Add(new XAttribute("nextRowKey", nextRowKeyStr));
                xmlData = StripNamespaces(root);


            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return xmlData;
        }

        /// <summary>
        /// This method will return complete data for selected entitySet.
        /// Get an XML element containing data from the specified table + container combination,
        /// filtering according to the filter criteria specified by the caller.
        /// </summary>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>An XML element containing the results of the query.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="WebException"></exception>
        /// <exception cref="Exception"></exception>
        public override XElement GetData(string realUri, string container, string tableName, string filter)
        {
            XElement tempGetData = null;
            string tempNextPartitionKey = string.Empty;
            string tempNextRowKey = string.Empty;

            //if (string.IsNullOrEmpty(container))
            //{
            //    throw new ArgumentNullException(AzureResources.ContainerCannotBeNull);
            //}

            try
            {
                //we don't need no page handling :)  
                tempGetData = GetData(realUri, container, tableName, filter, 0, null, null, 0);

                ////handling internal paging.
                //do
                //{
                //  if (tempGetData == null)
                //  {
                //      // 1000 is the max results Azure Table Storage allows per query
                //      tempGetData = GetData(realUri, container, tableName, filter, 1000, null, null);
                //      tempNextPartitionKey = tempGetData.Attribute("nextPartitionKey").Value;
                //      tempNextRowKey = tempGetData.Attribute("nextRowKey").Value;
                //  }
                //  else
                //  {
                //      // 1000 is the max results Azure Table Storage allows per query
                //      XElement tp = GetData(null, container, tableName, filter, 1000, tempNextPartitionKey, tempNextRowKey);
                //      tempGetData.Add(tp.Elements("properties"));

                //      // Update the partitionkey values at the top.
                //      tempGetData.SetAttributeValue("currentPartitionKey", tp.Attribute("currentPartitionKey").Value);
                //      tempGetData.SetAttributeValue("currentRowKey", tp.Attribute("currentRowKey").Value);
                //      tempGetData.SetAttributeValue("nextPartitionKey", tp.Attribute("nextPartitionKey").Value);
                //      tempGetData.SetAttributeValue("nextRowKey", tp.Attribute("nextRowKey").Value);

                //      tempNextPartitionKey = tp.Attribute("nextPartitionKey").Value;
                //      tempNextRowKey = tp.Attribute("nextRowKey").Value;
                //  }
                //}
                //while (!string.IsNullOrEmpty(tempNextPartitionKey) && !string.IsNullOrEmpty(tempNextRowKey));

                return tempGetData;
            }
            catch (Exception ex)
            {
                string sSource;
                string sLog;
                string sEvent;

                sSource = "ODP API";
                sLog = "Application";
                sEvent = ex.Message;

                if (!System.Diagnostics.EventLog.SourceExists(sSource))
                    System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);

                System.Diagnostics.EventLog.WriteEntry(sSource, sEvent);
                System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Warning, 234);

                throw;
            }
        }

        /// <summary>
        /// DAISY Plugin -  This method will return complete data for selected entitySet.
        /// Get an XML element containing data from the specified table + container combination,
        /// filtering according
        /// to the filter criteria specified by the caller.
        /// </summary>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>An XML element containing the results of the query.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="WebException"></exception>
        /// <exception cref="Exception"></exception>
        public override XDocument GetDataAsDaisy(string realUri, string container, string tableName, string filter)
        {
            XElement tempGetData = null;
            string tempNextPartitionKey = string.Empty;
            string tempNextRowKey = string.Empty;
            XDocument daisyDataXml = null;

            if (string.IsNullOrEmpty(container))
            {
                throw new ArgumentNullException(AzureResources.ContainerCannotBeNull);
            }

            try
            {
                //we don't need paging
                //tempGetData = GetData(realUri, container, tableName, filter, 0, null, null);
                tempGetData = GetDataAsXElement(realUri, container, tableName, filter);

                ////handling internal paging.
                //do
                //{
                //    if (tempGetData == null)
                //    {
                //        // 1000 is the max results Azure Table Storage allows per query
                //        tempGetData = GetData(realUri, container, tableName, filter, 1000, null, null);
                //        tempNextPartitionKey = tempGetData.Attribute("nextPartitionKey").Value;
                //        tempNextRowKey = tempGetData.Attribute("nextRowKey").Value;
                //    }
                //    else
                //    {
                //        // 1000 is the max results Azure Table Storage allows per query
                //        XElement tp = GetData(realUri, container, tableName, filter, 1000, tempNextPartitionKey, tempNextRowKey);
                //        tempGetData.Add(tp.Elements("properties"));

                //        // Update the partitionkey values at the top.
                //        tempGetData.SetAttributeValue("currentPartitionKey", tp.Attribute("currentPartitionKey").Value);
                //        tempGetData.SetAttributeValue("currentRowKey", tp.Attribute("currentRowKey").Value);
                //        tempGetData.SetAttributeValue("nextPartitionKey", tp.Attribute("nextPartitionKey").Value);
                //        tempGetData.SetAttributeValue("nextRowKey", tp.Attribute("nextRowKey").Value);

                //        tempNextPartitionKey = tp.Attribute("nextPartitionKey").Value;
                //        tempNextRowKey = tp.Attribute("nextRowKey").Value;
                //    }
                //}
                //while (!string.IsNullOrEmpty(tempNextPartitionKey) && !string.IsNullOrEmpty(tempNextRowKey));

                //Function to get the header information of a particular entity set
                AddMetadataToXElement(realUri, container, tempGetData);

                String xmlDataFromAzure = tempGetData.ToString();

                TextReader inputXMLDataForTrans = new StringReader(xmlDataFromAzure);
                XmlReader xmlReader = XmlReader.Create(inputXMLDataForTrans);

                // Translate dat in daisy format.
                DTBookTranslation.DTBook objBook = new DTBookTranslation.DTBook();

                TextReader validatedDTBookXml;
                //Xml got from azure environment is given as input for DTBook translation method
                string mappedPath = (HttpContext.Current != null) ? HttpContext.Current.Server.MapPath(PathDTD)
                    : Path.Combine(Environment.CurrentDirectory, PathDTD);
                validatedDTBookXml = objBook.TranslationOfAzureXml(xmlReader, mappedPath);
                daisyDataXml = XDocument.Load(validatedDTBookXml);
            }
            catch (Exception ex)
            {
                string sSource;
                string sLog;
                string sEvent;

                sSource = "ODP API";
                sLog = "Application";
                sEvent = ex.Message;

                if (!System.Diagnostics.EventLog.SourceExists(sSource))
                    System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);

                System.Diagnostics.EventLog.WriteEntry(sSource, sEvent);
                System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Warning, 234);

                throw;
            }
            return daisyDataXml;
        }

        /// <summary>
        /// Get colums of dataset
        /// </summary>
        /// <param name="container"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<string> GetColumns(string realUri, string container, string tableName)
        {
            // Set the filter
            //string tableNameFilter = "entityset eq '" + tableName + "'";

            //sl-king
            //TODO make this a reusable code (find all sfdgsdfgsdfgd)
            //EntitySet es = EntitySetRepository.GetEntitySet(container, entitySet);
            //since this is OData address looks like this .../service/category/entityset
            //metadata is at address .../service/category/$metadata
            string metadataUri = realUri;

            //first get rid of ?option1=...&option2=....
            var uriParts = metadataUri.Split('?');
            metadataUri = uriParts[0];
            if (metadataUri[metadataUri.Length - 1] != '/')//if it's not / terminated, let's terminate it
                metadataUri = metadataUri + "/";

            uriParts = metadataUri.Split('/');
            metadataUri = uriParts[0];
            for (int i = 1; i < uriParts.Length - 2; i++)
            {
                metadataUri += "/" + uriParts[i];
            }
            metadataUri += "/$metadata";


            XElement metaDataXML = GetMetadata(metadataUri, container, tableName);//"EntityMetadata");//, tableNameFilter);

            // Remove the unnecessary columns
            //var properties = metaDataXML.Elements("properties");
            //properties.Elements("entityset").Remove();
            //properties.Elements("entitykind").Remove();

            // Set the column list
            //var propertyMetaData = metaDataXML.Elements("properties").First().Elements();
            var propertyMetaData = metaDataXML.Element("EntityType").Elements("Property");
            List<string> columns = new List<string>();
            foreach (var property in propertyMetaData)
            {
                //columns.Add(property.Name.ToString());
                if (property.Attribute("Name").Value == "PartitionKey" || property.Attribute("Name").Value == "RowKey")
                    continue;
                columns.Add(property.Attribute("Name").Value);
            }
            return columns;
        }

        /// <summary>
        /// This method will return complete data for selected entitySet as xml
        /// </summary>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>Xml containing the results of the query.</returns>
        private XElement GetDataAsXElement(string realUri, string container, string tableName, string filter)
        {
            string root = string.Format("<Root tableName=\"{0}\" currentPartitionKey=\"\" currentRowKey=\"\" nextPartitionKey=\"\" nextRowKey=\"\" />", tableName);

            XElement tempGetData = XElement.Parse(root);
            //string tempNextPartitionKey = string.Empty;
            //string tempNextRowKey = string.Empty;

            //we don't need paging
            XElement tp = GetData(realUri, container, tableName, filter, 0, null, null, 0);// tempNextPartitionKey, tempNextRowKey);
            var entries = tp.Elements("entry");
            foreach (var entry in entries)
            {
                var props = entry.Element("content").Element("properties");

                //remove the OGDI inherited virtual columns
                var prop = props.Element("PartitionKey");
                if (prop != null)
                    prop.Remove();
                prop = props.Element("RowKey");
                if (prop != null)
                    prop.Remove();

                tempGetData.Add(props);
            }

            ////handling internal paging.
            //do
            //{
            //    // TODO Refactor paging handling. This loop logic is confusing.

            //    // 1000 is the max results Azure Table Storage allows per query
            //    XElement tp = GetData(realUri, container, tableName, filter, 1000, tempNextPartitionKey, tempNextRowKey);
            //    tempGetData.Add(tp.Elements("properties"));

            //    // Update the partitionkey values at the top.
            //    tempGetData.SetAttributeValue("currentPartitionKey", tp.Attribute("currentPartitionKey").Value);
            //    tempGetData.SetAttributeValue("currentRowKey", tp.Attribute("currentRowKey").Value);
            //    tempGetData.SetAttributeValue("nextPartitionKey", tp.Attribute("nextPartitionKey").Value);
            //    tempGetData.SetAttributeValue("nextRowKey", tp.Attribute("nextRowKey").Value);

            //    tempNextPartitionKey = tp.Attribute("nextPartitionKey").Value;
            //    tempNextRowKey = tp.Attribute("nextRowKey").Value;

            //}
            //while (!string.IsNullOrEmpty(tempNextPartitionKey) && !string.IsNullOrEmpty(tempNextRowKey));            

            return tempGetData;
        }

        private string GetElement(XElement e, string key)
        {
            return e.Element(key) != null ? e.Element(key).Value : string.Empty;
        }

        /// <summary>
        /// This method will return complete data for selected entitySet as csv formatted string
        /// </summary>
        /// <param name="realUri">URI of the entityset's data feed read from the metadata.</param>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>An string in csv format containing the results of the query.</returns>
        public override string GetdDataAsCsv(string realUri, string container, string tableName, string filter)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), "1"));

            List<string> columns = GetColumns(realUri, container, tableName);
            XElement xml = GetDataAsXElement(realUri, container, tableName, filter);

            try
            {
                StringBuilder sbAllEntities = new StringBuilder();

                foreach (string column in columns)
                {
                    sbAllEntities.Append(column);
                    sbAllEntities.Append(",");
                }
                sbAllEntities.Remove(sbAllEntities.Length - 1, 1);
                sbAllEntities.Append(Environment.NewLine);

                foreach (var element in xml.Elements("properties"))
                {
                    foreach (string column in columns)
                    {
                        string value = GetElement(element, column);
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Replace(',', ' ');
                            value = value.Replace('\n', ' ');
                            sbAllEntities.Append(value);
                        }
                        else
                        {
                            sbAllEntities.Append(string.Empty);
                        }
                        sbAllEntities.Append(",");
                    }
                    sbAllEntities.Remove(sbAllEntities.Length - 1, 1);
                    sbAllEntities.Append(Environment.NewLine);
                }
                System.Diagnostics.Trace.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), "2"));
                return sbAllEntities.ToString();
            }
            catch
            {
                return null;
            }

        }

        // GetMetaData internally calls GetData method. This method is introduced to keep ogdi code more readable.
        // Wherever we need to get table's metadata, instead of GetData, we now call GetMetaData.
        /// <summary>
        /// Gets details of header columns returning after quering container + tablename combination.
        /// </summary>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">table name to fetch metadata from</param>
        /// <param name="filter">Filter query value in string format</param>
        /// <returns>Details of header columns returning after quering container + tablename combination.</returns>
        public override XElement GetMetadata(string uri, string container, string tableName)//, string filter)//sl-king OData does not support filters
        {
            // 1000 is the max results Azure Table Storage allows per query
            //return GetData(uri, container, tableName, filter, 1000, null, null);

            //since this is OData compatible there's no localization, no filter etc.				
            //before Resources.MetaDataTableName was used for localized version
            XElement root = GetData(uri, container, "$metadata", null, 0, null, null, 0);

            ////////////////////////////////////////////////////////////////
            //sl-king:
            //OData $metadata does not support $filter, so we must filter it ourselves
            var cont = root.Element("EntityContainer");
            string etName = "";//name of the EntityType describing tableName
            var ents = cont.Elements("EntitySet");
            var tobeRemoved = new List<XElement>();
            foreach (var ent in ents)
            {
                string name = ent.Attribute("Name").Value;
                //sl-king: here's another patch, we lost namespaces in GetData, but we know that names look like this: "namespace.name", so let's just ignore namespace.
                var parts = name.Split('.');
                name = parts[parts.Length - 1];//we know the name is the last (may also be the first if no namespace is used
                if (name == tableName)
                {
                    etName = ent.Attribute("EntityType").Value;
                    parts = etName.Split('.');
                    etName = parts[parts.Length - 1];//this is what we need to identify below
                }
                else
                    tobeRemoved.Add(ent);
            }

            ents = root.Elements("EntityType");
            foreach (var ent in ents)
            {
                string name = ent.Attribute("Name").Value;
                var parts = name.Split('.');
                //sl-king: here's another patch, we lost namespaces in GetData, but we know that names look like this: "namespace.name", so let's just ignore namespace.
                name = parts[parts.Length - 1];//we know the name is the last (may also be the first if no namespace is used
                if (name != etName)
                    tobeRemoved.Add(ent);
                //else we just leave the correct one alone
            }

            //now we can safely remove filtered out items
            foreach (var ent in tobeRemoved)
                ent.Remove();
            ////////////////////////////////////////////////////////////////

            return root;
        }

        #endregion
    }
}

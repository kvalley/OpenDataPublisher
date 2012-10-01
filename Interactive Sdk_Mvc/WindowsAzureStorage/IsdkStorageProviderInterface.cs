using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.Configuration;
using System.IO;

namespace Odp.InteractiveSdk.Mvc
{
    public abstract class IsdkStorageProviderInterface
    {
        /// <summary>
        /// Gets the object of Service and returns it for accessing other methods.
        /// This property requires appSettings "Service" key entry in Configuration file.
        /// </summary>
        /// <param name="serviceUri">Data Service Url</param>
        /// <returns>Returns instance of the service to call service functionalities.</returns>
        public static IsdkStorageProviderInterface GetServiceObject(string serviceUri, string pathDTD)
        {
            try
            {
                // Get the dll name from the config file
                string serviceDll = ConfigurationManager.AppSettings["Service"];

                IsdkStorageProviderInterface service = null;
                if (!String.IsNullOrEmpty(serviceDll))
                {
                    var type = Type.GetType(serviceDll);
                    Object obj = Activator.CreateInstance(type, new object[] { serviceUri, pathDTD });

                    // Type cast the object to IServiceInterface type
                    service = obj as IsdkStorageProviderInterface;
                }

                // Return ogdiService
                return service;
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

                throw (ex);
            }
        }

        /// <summary>
        /// Gets details of header columns returning after quering container + tablename combination.
        /// </summary>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">table name to fetch metadata from</param>
        /// <param name="filter">Filter query value in string format</param>
        /// <returns>Details of header columns returning after quering container + tablename combination.</returns>
        public abstract XElement GetMetadata(string uri, string container, string tableName);

        /// <summary>
        /// Get an XML element containing data from the specified table + container combination, 
        /// filtering according to the filter criteria specified by the caller.
        /// </summary>
        /// <param name="uri">URI of the entityset's data feed read from the metadata.</param>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <param name="pageSize">Number of rows to be fetched</param>
        /// <param name="nextPartitionKey">PartionKey to fetch next partion data</param>
        /// <param name="nextRowKey">RowKey to fetch row information</param>
        /// <returns>An XML element containing the results of the query.</returns>
        public abstract XElement GetData(string uri, string container, string tableName,
                                          string filter, int pageSize,
                                          string nextPartitionKey, string nextRowKey,
                                                                                    int skip);

        /// <summary>
        /// This method will return complete data for selected entitySet.
        /// Get an XML element containing data from the specified table + container combination,
        /// filtering according to the filter criteria specified by the caller.
        /// </summary>
        /// <param name="uri">URI of the entityset's data feed read from the metadata.</param>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>An XML element containing the results of the query.</returns>
        public abstract XElement GetData(string uri, string container, string tableName, string filter);

        /// <summary>
        /// DAISY Plugin -  This method will return complete data for selected entitySet.
        /// Get an XML element containing data from the specified table + container combination,
        /// filtering according
        /// to the filter criteria specified by the caller.
        /// </summary>
        /// <param name="uri">URI of the entityset's data feed read from the metadata.</param>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>An XML element containing the results of the query.</returns>
        public abstract XDocument GetDataAsDaisy(string uri, string container, string tableName, string filter);

        /// <summary>
        /// This method will return complete data for selected entitySet as csv formatted string
        /// </summary>
        /// <param name="uri">URI of the entityset's data feed read from the metadata.</param>
        /// <param name="container">Alias of the container</param>
        /// <param name="tableName">EntitySet\Table name</param>
        /// <param name="filter">Filter criteria, in Azure Table Services query syntax.</param>
        /// <returns>An string in csv format containing the results of the query.</returns>
        public abstract string GetdDataAsCsv(string uri, string container, string tableName, string filter);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Odp.Data.Sql;
using System.Data;

namespace Odp.Data.Sql
{
    [ObsoleteAttribute("AppDomain.AppendPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead.http://go.microsoft.com/fwlink/?linkid=14202")]
    public static class sqlAppSettings
    {
        static sqlAppSettings()
        {
            RefreshAvailableEndpoints();
        }

        //for OData feed
        public static string DataServiceConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["dataserviceconnectionstring"];
            }
        }

        //for App specific metadata
        public static string AppMetadataConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["appmetadataconnectionstring"];
            }
        }

        public static Dictionary<string, sqlAvailableEndpoint> RefreshAvailableEndpoints()
        {
            var sql = sqlServerConnection.GetAppMetadataInstance();
            var reader = sql.GetReader("odpdatasources", "Name,Description,Disclaimer", "");
            var availableEndpoints = new Dictionary<string, sqlAvailableEndpoint>();

            foreach (DataRow row in reader.Rows)
            {
                string desc = row[1].ToString();
                string disc = row[2].ToString();
                var endpoint = new sqlAvailableEndpoint { alias = row[0].ToString(), description = !string.IsNullOrEmpty(desc) ? desc : "", disclaimer = !string.IsNullOrEmpty(desc) ? disc : "", storageaccountkey = "", storageaccountname = "" };
                availableEndpoints.Add(endpoint.alias, endpoint);

            }

            HttpContext.Current.Cache["AvailableEndpoints"] = availableEndpoints;
            return availableEndpoints;
        }

        public static string RootServiceNamespace
        {
            get
            {
                return "ODP";
            }
        }

        public static string OgdiConfigTableStorageAccountName
        {
            get
            {
                return ParseFromConnectionString(ConnectionStringElement.AccountName);
            }
        }

        //this is password
        public static string OgdiConfigTableStorageAccountKey
        {
            get
            {
                return ParseFromConnectionString(ConnectionStringElement.AccountKey);
            }
        }

        private enum ConnectionStringElement { AccountName, AccountKey }
        private static string ParseFromConnectionString(ConnectionStringElement element)
        {
            return "";
        }

        public static Dictionary<string, sqlAvailableEndpoint> EnabledStorageAccounts
        {
            get
            {
                var enabledStorageAccounts
                    = (Dictionary<string, sqlAvailableEndpoint>)HttpContext.Current.Cache["AvailableEndpoints"]
                    ?? RefreshAvailableEndpoints();

                return enabledStorageAccounts;
            }
        }

        public static sqlAvailableEndpoint GetAvailableEndpointByAccountName(string accountName)
        {
            return EnabledStorageAccounts.Values.Where(r => r.storageaccountname == accountName).FirstOrDefault();

        }

        private const string _remainderRouteDataValue = "remainder";

        public static string RemainderRouteDataValue
        {
            get
            {
                return _remainderRouteDataValue;
            }
        }

        private const string _remainderRoutePatternSnippet = "{*" + _remainderRouteDataValue + "}";

        public static string RemainderRoutePatternSnippet
        {
            get
            {
                return _remainderRoutePatternSnippet;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.DataServices
{
    public static class AppSettings
    {
        static AppSettings()
        {
            //RefreshAvailableEndpoints();
        }

        public static string RootServiceNamespace
        {
            get
            {
                //sl-king
                //uses azure but it is unused
                return "";
            }
        }

        public static string OgdiConfigTableStorageAccountName
        {
            get
            {
                return ParseFromConnectionString(ConnectionStringElement.AccountName);
            }
        }

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
            //sl-king
            //uses azure but it is unused
            return "";
        }

        public static string TableStorageBaseUrl
        {
            get
            {
                //sl-king
                //uses azure but it is unused
                return "";
                //return RoleEnvironment.GetConfigurationSettingValue("TableStorageBaseUrl");
            }
        }

        public static string BlobStorageBaseUrl
        {
            get
            {
                //sl-king
                //uses azure but it is unused
                return "";
                //return RoleEnvironment.GetConfigurationSettingValue("BlobStorageBaseUrl");
            }
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
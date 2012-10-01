using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Odp.Data.Sql;

namespace Odp.DataServices
{
    public class sqlTableStorageHttpHandlerBase
    {
        // Setup namespaces
        protected static XNamespace _nsAtom = XNamespace.Get("http://www.w3.org/2005/Atom");
        protected static XNamespace _nsm = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
        protected static XNamespace _nsd = XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices");

        // Setup namespace specific names
        protected static XName _entryXName = _nsAtom + "entry";
        protected static XName _contentXName = _nsAtom + "content";
        protected static XName _propertiesXName = _nsm + "properties";
        protected static XName _idXName = _nsAtom + "id";
        protected static XName _categoryXName = _nsAtom + "category";
        protected static XName _kmlSnippetXName = _nsd + "kmlsnippet";
        protected static XName _storageAccountNameXName = _nsd + "storageaccountname";
        protected static XName _storageAccountKeyXName = _nsd + "storageaccountkey";

        protected static readonly string _termNamespaceString = sqlAppSettings.RootServiceNamespace + ".{0}.{1}";

        protected WebRequest CreateTableStorageSignedRequest(HttpContext context,
                                                     string accountName, string storageAccountKey,
                                                     string requestUrl,
                                                     bool isAvailableEndpointsRequest)
        {
            return CreateTableStorageSignedRequest(context, accountName, storageAccountKey, requestUrl,
                                            isAvailableEndpointsRequest, false);
        }

        protected WebRequest CreateTableStorageSignedRequest(HttpContext context,
                                                             string accountName, string storageAccountKey,
                                                             string requestUrl,
                                                             bool isAvailableEndpointsRequest,
                                                             bool ignoreQueryOptions)
        {
            throw new Exception("Overlooked function CreateTableStorageSignedRequest");
            return null;
        }

        protected WebRequest CreateBlobStorageSignedRequest(string blobId, string ogdiAlias, string entitySet)
        {
            //sl-king
            //uses azure but it is unused
            throw new Exception("Overlooked function CreateBlobStorageSignedRequest");
            return null;
        }

        protected string ConvertToXmlSpecialChars(string str)
        {
            string result = str.Replace("<", "&lt;");
            result = result.Replace(">", "&gt;");
            result = result.Replace("\"", "&quot;");
            result = result.Replace("'", "&apos;");
            result = result.Replace("&", "&amp;");
            return result;
        }
    }
}

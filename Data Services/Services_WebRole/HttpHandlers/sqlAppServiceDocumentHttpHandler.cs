using System;
using System.Web.Routing;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Xml;

using System.Collections.Generic;
using System.Data;
using Odp.Data.Sql;

namespace Odp.DataServices
{
    public class sqlAppServiceDocumentHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {
        public string DataSource { get; set; }

        private const string START_SERVICEDOCUMENT_TEMPLATE =
@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
<service xml:base='{0}' xmlns:atom='http://www.w3.org/2005/Atom' xmlns:app='http://www.w3.org/2007/app' xmlns='http://www.w3.org/2007/app'>
	<workspace>
		<atom:title>Default</atom:title>
";
        //sl-king: most of the properties (only title original OGDI) were added for minimum interactive sdk mvc compatibility
        private const string COLLECTION_TEMPLATE =
@"		<collection href='{0}'>
			<atom:title>{0}</atom:title>
		</collection>
";
        private const string END_SERVICEDOCUMENT_TEMPLATE =
@"	</workspace>
</service>";

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        string MakeValidString(string value)
        {
            if (value != null)
                return value;
            else
                return "";
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!this.IsHttpGet(context))
            {
                this.RespondForbidden(context);
            }
            else
            {
                context.Response.AddHeader("DataServiceVersion", "1.0;");
                context.Response.CacheControl = "no-cache";
                context.Response.ContentType = "application/xml;charset=utf-8";

                var xmlBase = "http://" + context.Request.Url.Host + context.Request.Url.AbsolutePath;

                try
                {
                    var sql = sqlServerConnection.GetAppMetadataInstance();
                    string sWhere = (string.IsNullOrEmpty(DataSource)) ? "" : "odpdatasources.Name='" + DataSource + "'";
                    var reader = sql.GetEntitySetsMetaData(sWhere);

                    context.Response.Write(string.Format(START_SERVICEDOCUMENT_TEMPLATE, xmlBase));
                    foreach(DataRow row in reader.Rows)
                    {
                        context.Response.Write(string.Format(COLLECTION_TEMPLATE, row["EntitySet"].ToString()));
                    }
                    context.Response.Write(END_SERVICEDOCUMENT_TEMPLATE);
                }
                catch (Exception ex)
                {
                    context.Response.Write(ex.Message);
                    context.Response.StatusCode = 500;//internal server error (int)response.StatusCode;
                    context.Response.End();

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
                }
            }
        }

        #endregion
    }
}

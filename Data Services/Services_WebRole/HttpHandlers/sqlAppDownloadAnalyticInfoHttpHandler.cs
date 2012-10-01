using System;
using System.Web.Routing;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Xml;
using System.Data;
using Odp.Data.Sql;

namespace Odp.DataServices
{
    public class sqlAppDownloadAnalyticInfoHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {
        private const string START_SERVICEDOCUMENT_TEMPLATE =
    @"<?xml version='1.0' encoding='utf-8'?>
<downloadanalyticinfo>
";

        private const string ITEM_TEMPLATE =
    @"	<download entity_id=""{0}"" download_id=""{1}"" downloads_total=""{2}"" />
";

        private const string END_SERVICEDOCUMENT_TEMPLATE =
    @"</downloadanalyticinfo>
";

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
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

                var newInfo = context.Request.QueryString["new"] ?? ""; // eg "E86E6088-3FB3-42C5-B924-57C22D4DC064||23"
                var newKeys = newInfo.Split('|');  // eg ["E86E6088-3FB3-42C5-B924-57C22D4DC064","","23"]

                var sql = sqlServerConnection.GetAppMetadataInstance();

                // if the entitysetid and downloadid are set then insert new analytic row, if not just list analytics

                if (newKeys.Length == 3 && newKeys[1] == "")
                {
                    var entitysetID = newKeys[0]; // e.g. "eid=E86E6088-3FB3-42C5-B924-57C22D4DC064"
                    var downloadID = newKeys[2]; // e.g. "did=23"

                    // make sure the id of entity set and download actually exist 
                    var reader = sql.GetReader(
                        @"SELECT *
							FROM odpdownloads
							WHERE odpdownloads.id = '" + downloadID + "' AND odpdownloads.entityset_id = '" + entitysetID + "';"
                            );

                    // then insert the analytic info about the download
                    if (reader.Rows.Count > 0)
                    {
                        var id = reader.Rows[0].ToString();
                        var ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        var query = "INSERT INTO odpanalyticinfodownloads (entityset_id, download_id, download_date, ip) values ('" + entitysetID + "'," + downloadID + ", '" + DateTime.Now + "','" + ip + "');";
                        int n = sql.Execute(query);
                    }

                }
                else
                {
                    // not an update or an insert, just returning all the analytic information
                    context.Response.Write(START_SERVICEDOCUMENT_TEMPLATE);

                    var reader = sql.GetReader(
                        @"SELECT entityset_id, download_id, Count(*) as download_count
                            FROM [OpenDataPublisher].[dbo].[odpanalyticinfodownloads]
                            GROUP BY entityset_id, download_id;");

                    foreach(DataRow row in reader.Rows)
                    {
                        context.Response.Write(string.Format(ITEM_TEMPLATE,
                            sql.ObjToString(row["entityset_id"]),
                            sql.ObjToString(row["download_id"]),
                            sql.ObjToString(row["download_count"])
                        ));
                    }

                    //TODO implement some comments mechanism someday?
                    context.Response.Write(END_SERVICEDOCUMENT_TEMPLATE);
                }
            }
        }

        #endregion
    }
}

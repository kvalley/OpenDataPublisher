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
    public class sqlAppAnalyticInfoHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {
        private const string START_SERVICEDOCUMENT_TEMPLATE =
    @"<?xml version='1.0' encoding='utf-8'?>
<analyticinfo>
";

        private const string ITEM_TEMPLATE =
    @"	<entityset name=""{0}"" views_total=""{1}"" views_today=""{2}"" views_average=""{3}"" last_viewed=""{4}"" positive_votes=""{5}"" negative_votes=""{6}"" />
";

        private const string END_SERVICEDOCUMENT_TEMPLATE =
    @"</analyticinfo>
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

                var newInfo = context.Request.QueryString["new"] ?? "";//e.g. "dc||entityset"
                var updateInfo = context.Request.QueryString["update"] ?? "";//e.g. "dc||entityset"
                var keysNew = newInfo.Split('|');//["dc","","entityset"]
                var keysUpdate = updateInfo.Split('|');//["dc","","entityset"]
                if (keysNew.Length == 3 && keysNew[1] == "")
                {
                    var sql = sqlServerConnection.GetAppMetadataInstance();
                    //check if it really doesn't exist yet
                    var reader = sql.GetReader(
                        @"SELECT odpdatasources.Id AS DataSourceId
							FROM odpanalyticinfo
							INNER JOIN odpappmetadata ON entityset_id = odpappmetadata.id
							INNER JOIN odpdatasources ON odpappmetadata.datasource_id = odpdatasources.id
							WHERE odpdatasources.Name = '" + keysNew[0] + "' AND EntitySet = '" + keysNew[2] + "';"
                        );

                    if (reader.Rows.Count > 0)
                    {
                        //get the entity set id if it really exists
                        DataTable reader2 = sql.GetReader(
                            @"SELECT odpappmetadata.id
								FROM odpappmetadata
								INNER JOIN odpdatasources ON odpdatasources.id = datasource_id
								WHERE odpdatasources.Name = '" + keysNew[0] + "' AND EntitySet = '" + keysNew[2] + "';"
                                );
                        if (reader2.Rows.Count > 0)
                        {
                            var sql3 = sqlServerConnection.GetAppMetadataInstance();
                            var id = reader2.Rows[0][0].ToString();
                            int n = sql3.Execute("INSERT INTO odpanalyticinfo (entityset_id, views_total, views_today, views_average) values (" + id + ", 1, 1, 1);");
                        }
                    }
                }
                else
                    if (keysUpdate.Length == 3 && keysUpdate[1] == "")
                    {
                        var views_total = context.Request.QueryString["views_total"] ?? "";
                        var views_today = context.Request.QueryString["views_today"] ?? "";
                        var views_average = context.Request.QueryString["views_average"] ?? "";
                        var last_viewed = context.Request.QueryString["last_viewed"] ?? "";
                        var positive_votes = context.Request.QueryString["positive_votes"] ?? "";
                        var negative_votes = context.Request.QueryString["negative_votes"] ?? "";

                        var sql = sqlServerConnection.GetAppMetadataInstance();
                        //get the entity set id if it really exists
                        var reader = sql.GetReader(
                            @"SELECT odpappmetadata.id
							FROM odpappmetadata
							INNER JOIN odpdatasources ON odpdatasources.id = datasource_id
							WHERE odpdatasources.Name = '" + keysUpdate[0] + "' AND EntitySet = '" + keysUpdate[2] + "';"
                                );
                        if (reader.Rows.Count > 0)
                        {
                            var id = reader.Rows[0][0].ToString();
                            string values = "";
                            if (!string.IsNullOrEmpty(views_total))
                                values += (string.IsNullOrEmpty(values) ? "" : ", ") + "views_total=" + views_total;
                            if (!string.IsNullOrEmpty(views_today))
                                values += (string.IsNullOrEmpty(values) ? "" : ", ") + "views_today=" + views_today;
                            if (!string.IsNullOrEmpty(views_average))
                                values += (string.IsNullOrEmpty(values) ? "" : ", ") + "views_average=" + views_average;
                            if (!string.IsNullOrEmpty(last_viewed))
                                values += (string.IsNullOrEmpty(values) ? "" : ", ") + "last_viewed='" + last_viewed + "'";
                            if (!string.IsNullOrEmpty(positive_votes))
                                values += (string.IsNullOrEmpty(values) ? "" : ", ") + "positive_votes=" + positive_votes;
                            if (!string.IsNullOrEmpty(negative_votes))
                                values += (string.IsNullOrEmpty(values) ? "" : ", ") + "negative_votes=" + negative_votes;
                            if (!string.IsNullOrEmpty(values))
                            {
                                var sql2 = sqlServerConnection.GetAppMetadataInstance();
                                int n = sql2.Execute("UPDATE odpanalyticinfo SET " + values + " WHERE entityset_id=" + id + ";");
                            }
                        }
                    }
                    else
                    {
                        context.Response.Write(START_SERVICEDOCUMENT_TEMPLATE);

                        var sql = sqlServerConnection.GetAppMetadataInstance();
                        var reader = sql.GetReader(
                            @"SELECT *, odpdatasources.Name AS DataSource, odpappmetadata.EntitySet AS EntitySet
							FROM odpanalyticinfo
							INNER JOIN odpappmetadata ON entityset_id = odpappmetadata.id
							INNER JOIN odpdatasources ON odpappmetadata.datasource_id = odpdatasources.id;");

                        foreach(DataRow row in reader.Rows)
                        {
                            context.Response.Write(string.Format(ITEM_TEMPLATE,
                                sql.ObjToString(row["DataSource"]) + "||" + sql.ObjToString(row["EntitySet"]),
                                sql.ObjToString(row["views_total"]),
                                sql.ObjToString(row["views_today"]),
                                sql.ObjToString(row["views_average"]),
                                sql.ObjToString(row["last_viewed"]),
                                sql.ObjToString(row["positive_votes"]),
                                sql.ObjToString(row["negative_votes"])
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

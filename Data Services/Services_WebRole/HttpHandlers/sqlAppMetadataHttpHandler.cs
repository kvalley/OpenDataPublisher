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
    public class sqlAppMetadataHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
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
@"		<collection href='{10}'>
			<atom:entityid>{0}</atom:entityid>
			<atom:category>{1}</atom:category>
			<atom:datasource>{2}</atom:datasource>
			<atom:datasourcedescription>{5}</atom:datasourcedescription>
			<atom:entityset>{3}</atom:entityset>
			<atom:datasetname>{4}</atom:datasetname>
			<atom:technicalinformation></atom:technicalinformation>
			<atom:collectioninstruments></atom:collectioninstruments>
			<atom:datadictionaryvariables></atom:datadictionaryvariables>
			<atom:additionalinfo></atom:additionalinfo>
			<atom:description>{15}</atom:description>
			<atom:keywords></atom:keywords>
			<atom:links></atom:links>
			<atom:periodcovered></atom:periodcovered>
			<atom:geographiccoverage></atom:geographiccoverage>
			<atom:collectionmode></atom:collectionmode>
			<atom:updatefrequency></atom:updatefrequency>
			<atom:lastupdatedate>{11}</atom:lastupdatedate>
			<atom:releaseddate>{12}</atom:releaseddate>
			<atom:expireddate>{13}</atom:expireddate>
			<atom:metadataurl></atom:metadataurl>
			<atom:entitykind></atom:entitykind>
			<atom:downloadlinks>{6}</atom:downloadlinks>
			<atom:datasvclink>{7}</atom:datasvclink>
			<atom:datasvckmllink>{8}</atom:datasvckmllink>
			<atom:datasvckmltype>{9}</atom:datasvckmltype>
            <atom:datasetimage>{14}</atom:datasetimage>
		</collection>
";
        //@"    <collection href='{0}'>
        //				<atom:entityid>{1}</atom:entityid>
        //				<atom:name>{0}</atom:name>
        //				<atom:entitykind></atom:entitykind>
        //				<atom:category></atom:category>
        //				<atom:description></atom:description>
        //				<atom:source></atom:source>
        //				<atom:metadataurl></atom:metadataurl>
        //				<atom:entityset>{0}</atom:entityset>
        //				<atom:title>{0}</atom:title>
        //			</collection>
        //";
        // template for each download link, is inserted into the collection template above
        private const string DOWNLOAD_TEMPLATE =
@"	        <atom:downloadlink>
                <atom:downloadlinkname>{0}</atom:downloadlinkname>
                <atom:downloadlinkdescription>{1}</atom:downloadlinkdescription>
                <atom:downloadlinkurl>{2}</atom:downloadlinkurl>
                <atom:downloadlinkiconurl>{3}</atom:downloadlinkiconurl>
                <atom:downloadlinktype>{4}</atom:downloadlinktype>
                <atom:downloadinfourl>{5}</atom:downloadinfourl>
                <atom:downloadcount>{6}</atom:downloadcount>
                <atom:downloadid>{7}</atom:downloadid>
            </atom:downloadlink>
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
                    string sSQL = (string.IsNullOrEmpty(DataSource)) ? "" : "odpdatasources.Name='" + DataSource + "'";

                    var sql = sqlServerConnection.GetAppMetadataInstance();
                    var reader = sql.GetEntitySetsMetaData(sSQL);

                    string sSQLLink = "";
                    string linkDetails = "";

                    //var tables = sql.GetTables();
                    context.Response.Write(string.Format(START_SERVICEDOCUMENT_TEMPLATE, xmlBase));
                    foreach(DataRow row in reader.Rows)
                    {
                        linkDetails = "";

                        sSQLLink = (string.IsNullOrEmpty(row["EntityId"].ToString())) ? "" : "odpdownloads.entityset_id='" + row["EntityId"].ToString() + "'";

                        var sql2 = sqlServerConnection.GetAppMetadataInstance();
                        var linkreader = sql2.GetEntitySetsLinks(sSQLLink);

                        foreach(DataRow lrow in linkreader.Rows)
                        {
                            linkDetails += string.Format(DOWNLOAD_TEMPLATE,
                                "<![CDATA[" + lrow["Name"].ToString().ToString() + "]]>", // 0
                                "<![CDATA[" + lrow["Description"].ToString() + "]]>", // 1
                                "<![CDATA[" + lrow["Link"].ToString() + "]]>", // 2
                                "<![CDATA[" + lrow["IconLink"].ToString() + "]]>", // 3
                                "<![CDATA[" + lrow["FileType"].ToString() + "]]>", // 4
                                "<![CDATA[" + lrow["AdditionalInfoLink"].ToString() + "]]>", // 5
                                lrow["DownloadCount"].ToString(), // 6
                                lrow["ID"].ToString() // 7
                                );
                        }

                        context.Response.Write(string.Format(COLLECTION_TEMPLATE,
                            row["EntityId"].ToString(),//0
                            "<![CDATA[" + row["Category"].ToString() + "]]>",//1
                            "<![CDATA[" + row["Datasource"].ToString() + "]]>",//2
                            "<![CDATA[" + row["EntitySet"].ToString() + "]]>",//3
                            "<![CDATA[" + row["DatasetName"].ToString() + "]]>",//4
                            "<![CDATA[" + row["DatasourceDescription"].ToString() + "]]>",//5
                            //"<![CDATA["+reader["downloadlink"].ToString()+"]]>",//6
                            linkDetails,
                            "<![CDATA[" + row["datasvclink"].ToString() + "]]>",//7
                            "<![CDATA[" + row["datasvckmllink"].ToString() + "]]>",//8
                            row["datasvckmltype"].ToString(),//9
                            row["datasvclink"].ToString(),//10
                            sql.ObjToString(row["LastUpdateDate"]),//11
                            sql.ObjToString(row["ReleasedDate"]),//12
                            sql.ObjToString(row["ExpiredDate"]),//13
                            "<![CDATA[" + row["ImageLink"].ToString() + "]]>", //14
                            "<![CDATA[" + row["Description"].ToString() + "]]>" //15
                            ));
                    }
                    context.Response.Write(END_SERVICEDOCUMENT_TEMPLATE);
                }
                catch (Exception ex)
                {
                    Odp.Data.ErrorLog.WriteError(ex.Message);
                }
            }
        }

        #endregion
    }
}

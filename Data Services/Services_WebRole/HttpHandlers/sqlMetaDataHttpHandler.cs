using System;
using System.Web.Routing;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using Odp.Data.Sql;

namespace Odp.DataServices
{
    public class sqlMetaDataHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {
        public string OgdiAlias { get; set; }

        private readonly string START_DATASERVICES_TEMPLATE =
@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
<edmx:Edmx Version='1.0' xmlns:edmx='http://schemas.microsoft.com/ado/2007/06/edmx'>
  <edmx:DataServices>
    <Schema Namespace='ODP.{0}' xmlns:d='http://schemas.microsoft.com/ado/2007/08/dataservices' xmlns:m='http://schemas.microsoft.com/ado/2007/08/dataservices/metadata' xmlns='http://schemas.microsoft.com/ado/2007/05/edm'>
      <EntityContainer Name='{0}DataService' m:IsDefaultEntityContainer='true'>
";

        private readonly string ENTITYSET_TEMPLATE =
@"        <EntitySet Name='{0}' EntityType='ODP.{1}.{2}' />
";

        //        private const string END_ENTITYCONTAINER_TEMPLATE =
        //@"      </EntityContainer>
        //    </Schema>
        //";
        private const string END_ENTITYCONTAINER_TEMPLATE =
@"      </EntityContainer>
";

        //        private readonly string START_ENTITYTYPESCHEMA_TEMPLATE =
        //@"    <Schema Namespace='" + AppSettings.RootServiceNamespace + @".{0}' xmlns:d='http://schemas.microsoft.com/ado/2007/08/dataservices' xmlns:m='http://schemas.microsoft.com/ado/2007/08/dataservices/metadata' xmlns='http://schemas.microsoft.com/ado/2006/04/edm'>
        //";

        private const string START_ENTITYTYPE_TEMPLATE =
@"      <EntityType Name='{0}'>
        <Key>
          <PropertyRef Name='PartitionKey' />
          <PropertyRef Name='RowKey' />
        </Key>
        <Property Name='PartitionKey' Type='Edm.String' Nullable='false' />
        <Property Name='RowKey' Type='Edm.String' Nullable='false' />
";
        //@"      <EntityType Name='{0}'>
        //        <Key>
        //          <PropertyRef Name='PartitionKey' />
        //          <PropertyRef Name='RowKey' />
        //        </Key>
        //        <Property Name='PartitionKey' Type='Edm.String' Nullable='false' />
        //        <Property Name='RowKey' Type='Edm.String' Nullable='false' />
        //        <Property Name='Timestamp' Type='Edm.DateTime' Nullable='false' />
        //        <Property Name='entityid' Type='Edm.Guid' Nullable='false' />
        //";

        private const string START_PROPERTY_TEMPLATE =
@"        <Property Name='{0}' Type='{1}' Nullable='{2}' />
";

        private const string END_ENTITYTYPE_TEMPLATE =
@"      </EntityType>
";

        private const string END_ENTITYTYPESCHEMA_TEMPLATE =
@"    </Schema>
";

        private const string END_DATASERVICES_TEMPLATE =
@"  </edmx:DataServices>
</edmx:Edmx>";

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

                if (OgdiAlias.ToLower() != "sql")
                {
                    context.Response.Write("Endpoint '" + OgdiAlias + "' does not exist.");
                    context.Response.StatusCode = 404;//internal server error (int)response.StatusCode;
                    context.Response.End();
                    return;
                }

                try
                {
                    context.Response.Write(string.Format(START_DATASERVICES_TEMPLATE, this.OgdiAlias.ToLower()));

                    var sql = sqlServerConnection.GetDataServiceInstance();
                    var metadata = sql.GetTablesMetadata(/*filter,*/"", true);

                    //overview of all EntitySets
                    foreach (var data in metadata)
                        context.Response.Write(string.Format(ENTITYSET_TEMPLATE, data.name, this.OgdiAlias, data.name + "Item"));
                    context.Response.Write(END_ENTITYCONTAINER_TEMPLATE);

                    //these are properties from the template which we'll ignore if found
                    string[] fields = new string[] { "PartitionKey", "RowKey" };//, "Timestamp", "entityid"};

                    //a detailed data for each EntitySet
                    foreach (var data in metadata)
                    {
                        context.Response.Write(string.Format(START_ENTITYTYPE_TEMPLATE, data.name + "Item"));
                        foreach (var pdata in data.properties)
                        {
                            //to be able to work with prepared templates
                            //we have to get rid of entityid, which doesn't exist
                            if (fields.Any(s => s.Equals(pdata.name)))
                                continue;

                            context.Response.Write(string.Format(START_PROPERTY_TEMPLATE, pdata.name, pdata.dataType, pdata.nullable));
                        }
                        context.Response.Write(END_ENTITYTYPE_TEMPLATE);
                    }

                    context.Response.Write(END_ENTITYTYPESCHEMA_TEMPLATE);
                    context.Response.Write(END_DATASERVICES_TEMPLATE);
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

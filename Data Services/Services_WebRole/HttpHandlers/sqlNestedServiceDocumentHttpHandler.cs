using System;
using System.Linq;
using System.Net;
using System.Web;

namespace Odp.DataServices
{
    public class sqlNestedServiceDocumentHttpHandler : sqlTableStorageHttpHandlerBase, IHttpHandler
    {
        private const string START_SERVICEDOCUMENT_TEMPLATE =
@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
<service xml:base='{0}' xmlns:atom='http://www.w3.org/2005/Atom' xmlns:app='http://www.w3.org/2007/app' xmlns='http://www.w3.org/2007/app'>
	<workspace>
		<atom:title>Default</atom:title>
";

        private const string COLLECTION_TEMPLATE =
@"		<collection href='{0}'>
			<atom:title>{1}</atom:title>
			<accept>application/atomsvc+xml</accept>
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

        public void ProcessRequest(HttpContext context)
        {
            if (!this.IsHttpGet(context))
            {
                this.RespondForbidden(context);
            }
            else
            {
                //context.Response.Headers["DataServiceVersion"] = "1.0;";
                context.Response.AddHeader("DataServiceVersion", "1.0;");//VS integrated web server does not support .Headers[] =

                context.Response.CacheControl = "no-cache";
                context.Response.ContentType = "application/xml;charset=utf-8";

                var xmlBase = "http://" + context.Request.Url.Host + context.Request.Url.AbsolutePath;

                //var ta = CloudStorageAccount.FromConfigurationSetting("OgdiConfigConnectionString");
                //var ogdiConfigContext = new OgdiConfigDataServiceContext(ta.TableEndpoint.AbsoluteUri, ta.Credentials);

                try
                {
                    ////var list = ogdiConfigContext.AvailableEndpoints.ToList();
                    //var list = sqlAppSettings.RefreshAvailableEndpoints();

                    context.Response.Write(string.Format(START_SERVICEDOCUMENT_TEMPLATE, xmlBase));

                    context.Response.Write(string.Format(COLLECTION_TEMPLATE, "sql", "sql"));
                    //foreach (var item in list)
                    //{
                    //  //context.Response.Write(string.Format(COLLECTION_TEMPLATE,
                    //  //                                     item.alias, item.description));
                    //  context.Response.Write(string.Format(COLLECTION_TEMPLATE,
                    //                                       item.Value.alias, item.Value.description));
                    //}

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

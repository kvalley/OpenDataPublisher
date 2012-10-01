using System;
using System.Web.Routing;

namespace Odp.DataServices
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Add("V1MetaData", new Route("v1/{OgdiAlias}/$metadata/{*remainder}", new sqlMetaDataRouteHandler()));
            routes.Add("V1ServiceDocument", new Route("v1/{OgdiAlias}", new sqlServiceDocumentRouteHandler()));
            routes.Add("V1NestedServiceDocuments", new Route("v1", new sqlNestedServiceDocumentRouteHandler()));
            routes.Add("V1PrimaryRoute", new Route("v1/{OgdiAlias}/{EntitySet}/{*remainder}", new sqlEntitySetHandler()));//XML template instead of Azure XML
            routes.Add("V1AvailableEndpoints", new Route("v1/AvailableEndpoints", new sqlAvailableEndpointsHandler()));//XML template instead of Azure XML

            routes.Add("InteractiveMetadata", new Route("interactive/Metadata/{DataSource}/{*remainder}", new sqlAppMetadataRouteHandler()));
            routes.Add("InteractiveAvailableEndpoints", new Route("interactive/AvailableEndpoints/{*remainder}", new sqlAppAvailableEndpointsHandler()));//XML template instead of Azure XML
            routes.Add("InteractiveAnalyticInfo", new Route("interactive/AnalyticInfo/{*remainder}", new sqlAppAnalyticInfoRouteHandler()));
            routes.Add("InteractiveComments", new Route("interactive/Comments", new sqlAppCommentsRouteHandler()));
            routes.Add("InteractiveServiceDocument", new Route("interactive/EntitySets/{DataSource}/{*remainder}", new sqlAppServiceDocumentRouteHandler()));
            routes.Add("InteractiveDownloadAnalyticInfo", new Route("interactive/DownloadAnalyticInfo/{*remainder}", new sqlAppDownloadAnalyticInfoRouteHandler()));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
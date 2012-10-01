using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlAppMetadataRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var appMetadataHttpHandler = new sqlAppMetadataHttpHandler()
						{
                DataSource = requestContext.RouteData.Values["DataSource"] as string
						};
            return appMetadataHttpHandler;
        }

        #endregion
    }
}

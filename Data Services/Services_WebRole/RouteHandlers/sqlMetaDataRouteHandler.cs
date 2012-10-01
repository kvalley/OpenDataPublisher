using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlMetaDataRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var metaDataHttpHandler = new sqlMetaDataHttpHandler()
            {
                OgdiAlias = requestContext.RouteData.Values["OgdiAlias"] as string
            };


            return metaDataHttpHandler;
        }

        #endregion
    }
}

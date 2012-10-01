using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlServiceDocumentRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var serviceDocumentHttpHandler = new sqlServiceDocumentHttpHandler()
            {
                OgdiAlias = requestContext.RouteData.Values["OgdiAlias"] as string
            };


            return serviceDocumentHttpHandler;
        }

        #endregion
    }
}

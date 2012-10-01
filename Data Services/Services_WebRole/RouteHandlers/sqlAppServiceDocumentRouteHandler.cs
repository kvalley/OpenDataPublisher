using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlAppServiceDocumentRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var serviceDocumentHttpHandler = new sqlAppServiceDocumentHttpHandler()
            {
                DataSource = requestContext.RouteData.Values["DataSource"] as string
            };


            return serviceDocumentHttpHandler;
        }

        #endregion
    }
}

using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlAppDownloadAnalyticInfoRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpHandler = new sqlAppDownloadAnalyticInfoHttpHandler();
            return httpHandler;
        }

        #endregion
    }
}

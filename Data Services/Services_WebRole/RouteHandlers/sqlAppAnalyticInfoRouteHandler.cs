using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlAppAnalyticInfoRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpHandler = new sqlAppAnalyticInfoHttpHandler();
            return httpHandler;
        }

        #endregion
    }
}

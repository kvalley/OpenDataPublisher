using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlAppCommentsRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpHandler = new sqlAppCommentsHttpHandler();
            return httpHandler;
        }

        #endregion
    }
}

using System;
using System.Web.Routing;
using System.Web;

namespace Odp.DataServices
{
    public class sqlNestedServiceDocumentRouteHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {

            return new sqlNestedServiceDocumentHttpHandler();
        }

        #endregion
    }
}

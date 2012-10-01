using System.Web;
using System.Web.Routing;

namespace Odp.DataServices
{
    public class sqlAppAvailableEndpointsHandler : IRouteHandler
    {
        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            IHttpHandler handlerToReturn = null;
            {
                handlerToReturn = new sqlAppAvailableEndpointsHttpHandler();
            }

            return handlerToReturn;
        }

        #endregion
    }
}
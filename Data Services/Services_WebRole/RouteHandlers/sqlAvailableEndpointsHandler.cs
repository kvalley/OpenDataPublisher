using System.Web;
using System.Web.Routing;

namespace Odp.DataServices
{
    public class sqlAvailableEndpointsHandler : IRouteHandler
    {

        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            IHttpHandler handlerToReturn = null;
            {
                handlerToReturn = new sqlAvailableEndpointsHttpHandler();
            }

            return handlerToReturn;
        }

        #endregion
    }
}
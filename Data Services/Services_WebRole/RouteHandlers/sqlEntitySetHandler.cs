using System.Web;
using System.Web.Routing;

namespace Odp.DataServices
{
    public class sqlEntitySetHandler : IRouteHandler
    {
        private bool _isAvailableEndpointsRequest;

        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            _isAvailableEndpointsRequest = string.Compare(((Route)requestContext.RouteData.Route).Url, "v1/AvailableEndpoints", true) == 0;
            string remainder = requestContext.RouteData.Values["remainder"] as string;
            string ogdiAlias = requestContext.RouteData.Values["OgdiAlias"] as string;
            string entitySet = requestContext.RouteData.Values["EntitySet"] as string;

            return new sqlEntitySetHttpHandler() { _ogdiAlias = ogdiAlias, _entitySet = entitySet, _remainder = remainder };
        }

        #endregion
    }
}
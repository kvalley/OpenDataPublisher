using System.Web;

namespace Odp.DataServices
{
    public class NotFoundHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            this.RespondNotFound(context);
        }

        #endregion
    }
}

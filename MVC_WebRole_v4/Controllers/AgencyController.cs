using System.Web.Mvc;
using Odp.InteractiveSdk.Mvc.Models.Reports;
using System.Web;
using System.IO;
using System;

namespace Odp.InteractiveSdk.Mvc.Controllers
{
    public class AgencyController : Controller
    {
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
			return RedirectToAction("AgencyComments", "Comments");
        }
    }
}

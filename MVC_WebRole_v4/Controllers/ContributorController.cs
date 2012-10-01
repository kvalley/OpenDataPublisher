using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;
using Odp.InteractiveSdk.Mvc.Models;
using Odp.InteractiveSdk.Mvc.Repository;
using Odp.InteractiveSdk.Mvc.Models.Rating;
using Resources;

namespace Odp.InteractiveSdk.Mvc.Controllers
{
    public class ContributorController : Controller
    {
        private ContributorModel model = new ContributorModel();

        public ActionResult Index(string contributor)
        {
            var sql = Odp.Data.Sql.sqlServerConnection.GetDataServiceInstance();
            model = new ContributorModel();
            model.OtherModel = new DatasetListModel(0, 0, new OrderByInfo(), null, null);

            foreach (Container con in model.OtherModel.AllContainers)
            {
                if (con.Alias == contributor)
                {
                    model.Id = sql.GetDataSourceID(con.Alias);
                    model.Name = con.Alias;
                    model.Description = con.Description;
                    model.Disclaimer = con.Disclaimer;
                }
            }

            return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(ContributorModel model)
        {
            var sql = Odp.Data.Sql.sqlServerConnection.GetDataServiceInstance();
            this.model = model;

            if (model.Save == "Save")
            {
                if (model.Id == null)
                    sql.InsertContributor(model.Name, model.Description, model.Disclaimer);
                else 
                    sql.UpdateContributor(model.Id, model.Name, model.Description, model.Disclaimer);
            }
            else
            {
                return RedirectToAction("DataSetList", "DataCatalog");
            }

            return RedirectToAction("DataSetList", "DataCatalog");
        }
    }
}

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
    public class CategoryController : Controller
    {
        private CategoryModel model = new CategoryModel();

        public ActionResult Index(string category)
        {
            var sql = Odp.Data.Sql.sqlServerConnection.GetDataServiceInstance();
            model = new CategoryModel();
            model.OtherModel = new DatasetListModel(0, 0, new OrderByInfo(), null, null);

            model.Id = sql.GetCategoryID(category);
            model.Name = category;

            return View("Index", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(CategoryModel model)
        {
            var sql = Odp.Data.Sql.sqlServerConnection.GetDataServiceInstance();
            this.model = model;

            if (model.Save == "Save")
            {
                if (model.Id == null)
                    sql.InsertCategory(model.Name);
                else
                    sql.UpdateCategory(model.Id, model.Name);
            }
            else
            {
                return RedirectToAction("DataSetList", "DataCatalog");
            }

            return RedirectToAction("DataSetList", "DataCatalog");
        }
    }
}

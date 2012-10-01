using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Odp.UserInterface.Models;
using Odp.UserInterface.App_Resources;

namespace Odp.UserInterface.Controllers
{
    [HandleError]
    public class DataCatalogController : Controller
    {
        #region Private Members

        public DataCatalogModel viewDataModel = new DataCatalogModel();

        #endregion

        #region DataCatalog Static Members

        // Hashtable to store ContainerAlias & ContainerDisclaimer
        static Hashtable hashAliasDisclaimer;

        #endregion

        #region Public Methods

        public ActionResult DataSetList()
        {
            var model = new DatasetListModel(0, 0, new OrderByInfo(), null, null);
            model.OtherModel = viewDataModel;

            return View("DataSetList", System.Configuration.ConfigurationManager.AppSettings["MasterPageName"], model);
        }

        public ActionResult DataSets(int pageSize, int pageNumber, string orderField, string orderType, Filter filter)
        {
            var direction = SortDirection.Desc;
            if (orderType != null && orderType == "Asc")
                direction = SortDirection.Asc;

            var field = Field.Name;
            if (orderField != null)
            {
                switch (orderField)
                {
                    case "Image":
                        field = Field.Image;
                        break;
                    case "Name":
                        field = Field.Name;
                        break;
                    case "Description":
                        field = Field.Description;
                        break;
                    case "Category":
                        field = Field.Category;
                        break;
                    case "Status":
                        field = Field.Status;
                        break;
                    case "Date":
                        field = Field.Date;
                        break;
                    case "Rating":
                        field = Field.Rating;
                        break;
                    case "Views":
                        field = Field.Views;
                        break;
                    default:
                        field = Field.Name;
                        break;
                }
            }

            IEnumerable<string> containerAliases = null;
            List<Func<EntitySet, bool>> filters = null;

            if (filter != null)
            {
                filters = new List<Func<EntitySet, bool>>();

                if (filter.DataSources != null && filter.DataSources.Length > 0)
                    containerAliases = filter.DataSources;

                if (filter.Statuses != null && filter.Statuses.Length == 1)
                    filters.Add(filter.Statuses[0].ToLower() == "planned"
                        ? (Func<EntitySet, bool>)(set => set.IsEmpty)
                        : set => !set.IsEmpty);

                if (filter.PublishingDates != null && filter.PublishingDates.From != DateTime.MinValue)
                    filters.Add(set => set.LastUpdateDate >= filter.PublishingDates.From);

                if (filter.PublishingDates != null && filter.PublishingDates.To != DateTime.MinValue)
                    filters.Add(set => set.LastUpdateDate <= filter.PublishingDates.To);

                if (filter.Categories != null && filter.Categories.Length > 0)
                    filters.Add(set => filter.Categories.Contains(set.CategoryValue));

                if (!string.IsNullOrEmpty(filter.Keywords))
                {
                    var filterKeywords = filter.Keywords.ToLower().Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    filters.Add(
                        set =>
                            (set.Keywords != null && ((Func<string, bool>)(setKeywords => filterKeywords.Any(setKeywords.Contains)))(set.Keywords.ToLower())) ||
                            (set.Name != null && ((Func<string, bool>)(setKeywords => filterKeywords.Any(setKeywords.Contains)))(set.Name.ToLower())) ||
                            (set.Description != null && ((Func<string, bool>)(setKeywords => filterKeywords.Any(setKeywords.Contains)))(set.Description.ToLower()))
                        );
                }
            }

            var model = new DatasetListModel(pageSize, pageNumber, new OrderByInfo { Direction = direction, Field = field }, containerAliases, filters);
            
            return View(model);
        }

        public ActionResult GetListDataJSON()
        {
            var data = (CommonListData)Session["DatasetListData"];
            if (data == null)
            {
                data = new CommonListData();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }

    public class Filter
    {
        public string[] Categories { get; set; }
        public string[] DataSources { get; set; }
        public string Keywords { get; set; }
        public string[] Statuses { get; set; }
        public string[] FileTypes { get; set; }
        public DatePeriodFilter PublishingDates { get; set; }
    }

    public class DatePeriodFilter
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}

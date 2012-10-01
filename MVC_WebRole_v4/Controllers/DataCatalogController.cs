/* 
 *   Copyright (c) Microsoft Corporation.  All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Microsoft Corporation nor the names of its contributors 
 *       may be used to endorse or promote products derived from this software
 *       without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE REGENTS AND CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Odp.InteractiveSdk.Mvc.Models;
using Resources;

namespace Odp.InteractiveSdk.Mvc.Controllers
{
    [HandleError]
    public class DataCatalogController : Controller
    {

        #region Private Members

        private DataCatalogModel viewDataModel = new DataCatalogModel();

        #endregion

        #region DataCatalog Static Members

        // Hashtable to store ContainerAlias & ContainerDisclaimer
        static Hashtable hashAliasDisclaimer;

        #endregion

        #region DataCatalog Public Methods

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

        /// <summary>
        /// This actions returns the legal disclaimer text in json format
        /// </summary>
        /// <param name="containerAlias">Container name in string format</param>
        /// <returns>Returns JsonResult of the Legal Disclaimer text</returns>
        public JsonResult ReturnLegalDisclaimerForThisAlias(string containerAlias)
        {
            HideError();

            // Declare a variable of type List<string>
            List<string> legalDisclaimer = null;

            // Gets Legal Disclaimer for the passed containerAlias
            try
            {
                // Define LegalDisclaimer
                legalDisclaimer = new List<string>();

                // Add to Legal Disclaimer to the LegalDisclaimer
                legalDisclaimer.Add(hashAliasDisclaimer[containerAlias].ToString());
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }

            return new JsonResult
            {
                Data = legalDisclaimer,
            };
        }

        #endregion

        #region DataCatalog Private Methods

        /// <summary>
        /// This method is called when error/exception occurs
        /// This method will set the value of viewDataModel.ErrorLine1
        /// and viewDataModel.ErrorLine2
        /// </summary>
        /// <param name="detail">details of an error</param>
        private void ShowError(string detail)
        {
            // Set the error ViewData
            viewDataModel.ErrorLine1 = UIConstants.GC_ErrorString;
            viewDataModel.ErrorLine2 = detail;
        }

        /// <summary>
        /// This method is called when a page is newly loaded
        /// This method will reset the value of viewDataModel.ErrorLine1
        /// and viewDataModel.ErrorLine2
        /// </summary>
        private void HideError()
        {
            // Reset the error ViewData
            viewDataModel.ErrorLine1 = string.Empty;
            viewDataModel.ErrorLine2 = string.Empty;
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

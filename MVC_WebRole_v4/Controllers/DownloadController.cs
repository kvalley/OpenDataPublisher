using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Odp.InteractiveSdk.Mvc.Models;
using Odp.InteractiveSdk.Mvc.Repository;
using System.Collections.Specialized;



namespace Odp.InteractiveSdk.Mvc.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        private readonly DataBrowserModel viewDataModel = new DataBrowserModel();

        public ActionResult Index(string container, string entitySetName, string downloadID)
        {
            // increment the download
            EntitySet entitySet = EntitySetRepository.GetEntitySet(container, entitySetName);

            viewDataModel.EntitySetWrapper = new EntitySetWrapper()
            {
                EntitySet = entitySet
            };

            viewDataModel.EntitySetWrapper.RegisterDownload(downloadID);


            // get the download information
            
            // the download link information is there but it's in a list and I'm not sure which one to grab. The id is not in the list to match
            // just retrieve the download id?  Can match any other way? 

            var downloadLink = "";
            // loop through download link list
            foreach (OrderedDictionary download in entitySet.DownloadLinks)
            {
                if (download["ID"].ToString() == downloadID) { 
                    downloadLink = download["Link"].ToString();
                    break;
                }
            }
            

            // send them off to the download
            return Redirect(downloadLink);
        }

    }
}

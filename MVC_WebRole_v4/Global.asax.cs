using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Odp.InteractiveSdk.Mvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "LoadSampleCodeDataView",
                "DataBrowser/LoadSampleCodeDataView/",
                new { controller = "DataBrowser", action = "LoadDataViewSampleCode" }
                );
            routes.MapRoute(
                "LoadSampleCodeMapView",
                "DataBrowser/LoadSampleCodeMapView/",
                new { controller = "DataBrowser", action = "LoadMapViewSampleCode" }
                );
            routes.MapRoute(
                "LoadSampleCodeBarChartView",
                "DataBrowser/LoadSampleCodeBarChartView/",
                new { controller = "DataBrowser", action = "LoadBarChartSampleCode" }
                );
            routes.MapRoute(
                "LoadSampleCodePieChartView",
                "DataBrowser/LoadSampleCodePieChartView/",
                new { controller = "DataBrowser", action = "LoadPieChartSampleCode" }
                );
            routes.MapRoute(
                "DataBrowserPaging",
                "DataBrowser/DataBrowserPaging/",
                new { controller = "DataBrowser", action = "PagingClicked" }
                );
            routes.MapRoute(
                "DataBrowserRun",
                "DataBrowser/DataBrowserRun/",
                new { controller = "DataBrowser", action = "RunButtonClicked" }
                );
            routes.MapRoute(
                "DataBrowserRunBarChart",
                "DataBrowser/DataBrowserRunBarChart/",
                new { controller = "DataBrowser", action = "RunBarChartButtonClicked" }
                );
            routes.MapRoute(
                "DataBrowserRunPieChart",
                "DataBrowser/DataBrowserRunPieChart/",
                new { controller = "DataBrowser", action = "RunPieChartButtonClicked" }
                );
            routes.MapRoute(
                "DataBrowserError",
                "DataBrowser/DataBrowserError/",
                new { controller = "DataBrowser", action = "ShowClientsideError" }
                );
            routes.MapRoute(
                "DataBrowser",
                "DataBrowser/{container}/{entitySetName}",
                new { controller = "DataBrowser", action = "Index", container = "", entitySetName = "" }
                );
            routes.MapRoute(
                "Download",
                "Download/{container}/{entitySetName}/{downloadID}",
                new { controller = "Download", action = "Index", container = "", entitySetName = "" }
                );
            routes.MapRoute(
                "VoteRoute",
                "Rates/{action}/{itemKey}",
                new { controller = "Rates", action = "Index", itemKey = "" }
                );


            routes.MapRoute(
                "LoadLegalDisclaimerMapRoute",
                "DataCatalog/LoadLegalDisclaimerMapRoute/{containerAlias}",
                new { controller = "DataCatalog", action = "ReturnLegalDisclaimerForThisAlias", containerAlias = "" }
                );
            routes.MapRoute(
                "LoadDataCatalogMapRoute",
                "DataCatalog/LoadDataCatalogMapRoute/{containerAlias}/{entitySetName}",
                new { controller = "DataCatalog", action = "LoadDataCatalogByContainerAlias", containerAlias = "", entitySetName = "" }
                );
            routes.MapRoute(
                "LoadDataCatalogEntitySets",
                "DataCatalog/LoadDataCatalogEntitySets/{containerAlias}/{categoryName}",
                new { controller = "DataCatalog", action = "LoadEntitySetsByCategory", containerAlias = "", categoryName = "" }
                );


            routes.MapRoute(
                "Load",                                                                               // Route name
                "DataLoader/{container}/{entitySetName}",                                                   // URL with parameters
                new { controller = "DataLoader", action = "Index", container = "", entitySetName = "" }     // Parameter defaults
                );
            routes.MapRoute(
                "Category",                                                                               // Route name
                "Category/{category}",                                                   // URL with parameters
                new { controller = "Category", action = "Index", category = "" }     // Parameter defaults
                );
            routes.MapRoute(
                "Contributor",                                                                               // Route name
                "Contributor/{contributor}",                                                   // URL with parameters
                new { controller = "Contributor", action = "Index", contributor = "" }     // Parameter defaults
                );
            routes.MapRoute(
                "Login",                                                                               // Route name
                "Login/",                                                   // URL with parameters
                new { controller = "Login", action = "Index" }     // Parameter defaults
                );


            routes.MapRoute(
                "ControllerAction",
                "{controller}/{action}",
                new { controller = "DataCatalog", action = "DataSetList" }
                );
            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "DataCatalog", action = "DataSetList", id = "" }  // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory("Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();
        }
    }
}
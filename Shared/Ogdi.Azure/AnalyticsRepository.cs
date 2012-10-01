using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.Data
{
    static public class AnalyticsRepository
    {
        static public void RegisterView(String itemKey, String url, String user)
        {
            DatasetInfoDataSource datasetInfoDataSource = new DatasetInfoDataSource();
            ViewDataSource viewDS = new ViewDataSource();
            datasetInfoDataSource.IncrementView(itemKey);
        }

        static public void RegisterDownload(String itemKey)
        {
            DatasetInfoDataSource datasetInfoDataSource = new DatasetInfoDataSource();
            ViewDataSource viewDS = new ViewDataSource();
            datasetInfoDataSource.RegisterDownload(itemKey);
        }
    }
}

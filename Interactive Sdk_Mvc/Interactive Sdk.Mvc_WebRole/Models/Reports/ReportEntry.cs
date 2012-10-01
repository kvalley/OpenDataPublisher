using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Microsoft.WindowsAzure.StorageClient;

namespace Odp.InteractiveSdk.Mvc.Models.Reports
{
    public class ReportEntry
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
    }
}

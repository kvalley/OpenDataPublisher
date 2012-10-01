using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Microsoft.WindowsAzure.StorageClient;

using Ogdi.InteractiveSdk.Mvc;

namespace Ogdi.InteractiveSdk.Mvc.Models.Rating
{
    public class ViewEntry: sqlTableServiceEntity
    {
        public ViewEntry()
        {
            this.RowKey = Guid.NewGuid().ToString();
            this.PartitionKey = "Views";
        }
        public string DatasetId
        {
            get;
            set;
        }
        public int Views { get; set; }
    }
}

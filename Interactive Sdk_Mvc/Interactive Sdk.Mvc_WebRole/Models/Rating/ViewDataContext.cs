using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Microsoft.WindowsAzure.StorageClient;
//using Microsoft.WindowsAzure;

using Ogdi.InteractiveSdk.Mvc;

namespace Ogdi.InteractiveSdk.Mvc.Models.Rating
{
    public class ViewDataContext : sqlTableServiceContext
    {
        public ViewDataContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }

        public IQueryable<ViewEntry> ViewEntry
        {
            get
            {
                return this.CreateQuery<ViewEntry>("ViewEntry");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Odp.Data.Sql;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

using Ogdi.InteractiveSdk.Mvc;

namespace Ogdi.InteractiveSdk.Mvc.Models.Rating
{
    public class ViewDataContext : sqlTableServiceContext
    {
        public ViewDataContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress)
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

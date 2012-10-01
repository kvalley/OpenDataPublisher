using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.InteractiveSdk.Mvc.Models.Request
{
    public class RequestDataContext : sqlTableServiceContext
    {
        public RequestDataContext(string baseAddress)//, StorageCredentials credentials)
            : base(baseAddress)
        {
        }

        public IQueryable<RequestEntry> Requests
        {
            get
            {
                return this.CreateQuery<RequestEntry>("Requests");
            }
        }
    }
}

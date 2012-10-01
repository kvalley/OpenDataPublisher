using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.UserInterface
{
    public class RateDataContext : sqlTableServiceContext
    {
        //public RateDataContext(string baseAddress, StorageCredentials credentials)
        public RateDataContext(string baseAddress)
            : base(baseAddress)
        {
        }

        public IQueryable<RateEntry> RateEntry
        {
            get
            {
                //TODO when all this wil be configured in some way, until then we can't do anything
                return null;
            }
        }
    }
}

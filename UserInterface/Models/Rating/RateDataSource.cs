using System;
using System.Collections.Generic;
using System.Linq;

namespace Odp.UserInterface
{
    public class RateDataSource
    {
        //private static CloudStorageAccount storageAccount;
        private RateDataContext context;

        static RateDataSource()
        {
        }

        public RateDataSource()
        {
        }

        public void AddVote(RateEntry item)
        {
        }

        public IQueryable<RateEntry> SelectAll()
        {
            return this.context.RateEntry;
        }
    }
}

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
    public class RateEntry : sqlTableServiceEntity
    {
        public DateTime RateDate { get; set; }
        public string User { get; set; }
        public int RateValue { get; set; }
        public String ItemKey { get; set; }

        public RateEntry()
        {
            this.RowKey = Guid.NewGuid().ToString();
        }

    }
}

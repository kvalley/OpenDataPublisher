using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;

namespace Odp.Data.Views
{
    public class ViewEntry : sqlTableServiceEntity
    {
        public DateTime Date { get; set; }
        public string User { get; set; }
        public String ItemKey { get; set; }
        public String RequestedUrl { get; set; }
        
        public ViewEntry()
        {
            this.RowKey = Guid.NewGuid().ToString(); 
            this.PartitionKey = "views";
        }
    }
}

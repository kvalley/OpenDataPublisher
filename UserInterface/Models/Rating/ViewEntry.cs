using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Odp.Data.Sql;
using Ogdi.UserInterface;

namespace Ogdi.UserInterface
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.UserInterface.Models.Request
{
    public class RequestEntry : sqlTableServiceEntity
    {        
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PostedDate { get; set; }
        public string Links { get; set; }
        public string DatasetLink { get; set; }
        public int Comments {get; set; }

        public RequestEntry()
        {
            this.RowKey = Guid.NewGuid().ToString();
            this.PartitionKey = "Rate";
        }
                
        public SelectList GetAvailableStatuses()
        {
            return new SelectList(new string[] { "New", "In Progress", "Completed" }, Status);
        }
    }
}

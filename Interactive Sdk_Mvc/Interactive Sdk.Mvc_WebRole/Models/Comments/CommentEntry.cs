using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.InteractiveSdk.Mvc.Models.Comments
{
    public class CommentEntry : sqlTableServiceEntity
    {
        public string DatasetId { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
        public DateTime PostedOn { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public bool Notify { get; set; }
        public string ParentType { get; set; }

        public CommentEntry()
        {
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}

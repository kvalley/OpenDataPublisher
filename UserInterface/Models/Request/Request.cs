using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Odp.UserInterface;

namespace Odp.UserInterface.Models.Request
{
    public class Request
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
        public int PositiveVotes { get; set; }
        public int NegativeVotes { get; set; }
        public int Views { get; set; }
        public int Comments {get; set; }
        public string RequestID { get; set; }
        
        public SelectList GetAvailableStatuses()
        {
            return new SelectList(new string[] { "New", "In Progress", "Completed" }, Status);
        }

        public String ItemKey
        {
            get
            {
                return Helper.GenerateRequestKey(this.RequestID);
            }
        }
    }

}

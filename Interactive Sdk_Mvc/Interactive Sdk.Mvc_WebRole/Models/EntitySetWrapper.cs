using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.InteractiveSdk.Mvc.Models
{
    public class EntitySetWrapper
    {
        public EntitySet EntitySet { get; set; }
        public int Rating
        {
            get
            {
                return PositiveVotes - NegativeVotes;
            }
        }
        public int PositiveVotes { get; set; }
        public int NegativeVotes { get; set; }
        public int Views { get; set; }

        //a little hack, but it does the job of incrementing the number of views even for download only entity sets
        public void IncrementView()
        {
            string baseUri = System.Configuration.ConfigurationManager.AppSettings["serviceUri"];
            AnalyticsRepository.RegisterView(EntitySet.ContainerAlias + "||" + EntitySet.EntitySetName, baseUri, "");

        }

        public void RegisterDownload(string downloadID)
        {
            string baseUri = System.Configuration.ConfigurationManager.AppSettings["serviceUri"];
            AnalyticsRepository.RegisterDownload(EntitySet.EntityId + "||" + downloadID);

        }
    }
}

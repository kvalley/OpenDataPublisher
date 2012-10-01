using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.InteractiveSdk.Mvc.Models
{
    public class HomeModel
    {

        private List<Odp.InteractiveSdk.Mvc.Models.BlogAndAnnouncement> newsData;

        public List<Odp.InteractiveSdk.Mvc.Models.BlogAndAnnouncement> NewsData
        {
            get { return newsData; }
            set { newsData = value; }
        }

    }
}

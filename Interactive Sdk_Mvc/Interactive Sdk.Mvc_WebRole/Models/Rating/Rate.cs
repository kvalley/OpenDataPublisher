using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.InteractiveSdk.Mvc.Models.Rating
{
    public class Rate
    {
        public DateTime RateDate { get; set; }
        public string User { get; set; }
        public String ItemKey { get; set; }
        public int RateValue { get; set; }
    }

    public class VoteResults
    {
        public int Positive { get; set; }
        public int Negative { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Odp.InteractiveSdk.Mvc.Repository;

namespace Odp.InteractiveSdk.Mvc.Models.Rating
{
    public class RateInfo
    {
        public RateInfo(string itemKey)
        {
            ItemKey = itemKey;
        }

        public RateInfo(string itemKey, int positiveRates, int negativeRates)
            : this(itemKey)
        {
            this.positiveRates = positiveRates;
            this.negativeRates = negativeRates;
            this.ReadOnly = true;
        }

        public string ItemKey { get; set; }
        public bool ReadOnly { get; set; }
        private int? positiveRates;
        public int PositiveRates
        {
            get
            {
                if (!positiveRates.HasValue)
                {
                    RefreshRating();
                }
                return positiveRates.Value;
            }
        }
        private int? negativeRates;
        public int NegativeRates
        {
            get
            {
                if (!negativeRates.HasValue)
                {
                    RefreshRating();
                }
                return negativeRates.Value;
            }
        }

        private void RefreshRating()
        {
            positiveRates = 0;// vr.Positive;
            negativeRates = 0;// vr.Negative;
        }

        public bool HasUserVoted(HttpRequest req)
        {
            return false;
        }

        private string GetCurrentUser(HttpRequest req)
        {
            if (!req.IsAuthenticated)
            {
                return req.UserHostAddress;
            }
            else
            {
                return req.LogonUserIdentity.Name;
            }
        }
    }
}

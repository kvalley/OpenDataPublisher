using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;
using Odp.UserInterface.Models;

namespace Odp.UserInterface.Repository
{
    public class RatingRepository
    {
        static public void AddVote(Rate item)
        {
            RateDataSource rateDS = new RateDataSource();
            DatasetInfoDataSource datasetInfoDS = new DatasetInfoDataSource();

            rateDS.AddVote(new RateEntry()
            {
                RowKey = Guid.NewGuid().ToString(),
                ItemKey = item.ItemKey,
                PartitionKey = "rates",
                RateDate = item.RateDate,
                RateValue = item.RateValue,
                User = item.User,
            });

            datasetInfoDS.IncrementVote(item.ItemKey, item.RateValue);
        }

        static public VoteResults GetVoteResults(string itemKey)
        {
            DatasetInfoDataSource datasetInfoDS = new DatasetInfoDataSource();

            AnalyticInfo dInfo = datasetInfoDS.GetAnalyticSummary(itemKey);

            return new VoteResults()
            {
                Positive = dInfo.PositiveVotes,
                Negative = dInfo.NegativeVotes,
            };
        }

        static public bool HasUserVoted(String itemKey, string userName)
        {
            RateDataSource rateDS = new RateDataSource();

            var results = from g in rateDS.SelectAll()
                          where g.ItemKey == itemKey
                          && g.User == userName
                          select g;

            return results.FirstOrDefault() == null ? false : true;
        }

        static public void IncrementView(String itemKey)
        {
            DatasetInfoDataSource datasetInfoDS = new DatasetInfoDataSource();
            datasetInfoDS.IncrementView(itemKey);
        }
    }
}

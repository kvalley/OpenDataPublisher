using System;
using System.Linq;
using System.Collections.Generic;

using Odp.Data;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.Data.DataSets
{
    public sealed class DatasetInfoDataSource
    {
        //private static readonly CloudStorageAccount StorageAccount;
        private static readonly object Mutex = new object();

        private readonly DatasetInfoDataContext _context;

        static DatasetInfoDataSource()
        {
        }

        public DatasetInfoDataSource()
        {
            _context = new DatasetInfoDataContext(System.Configuration.ConfigurationManager.AppSettings["serviceUri"]);
        }

        public void IncrementView(string itemKey)
        {
            lock (Mutex) // NOTE: This synchronization is not enough since there may be several instances of roles.
            {
                var result = GetOrCreateAnalyticInfo(itemKey);


                if (result.last_viewed.Date == DateTime.Today)
                {
                    result.last_viewed = DateTime.Now;
                    result.views_today += 1;
                    result.views_total += 1;
                }
                else
                {
                    result.last_viewed = DateTime.Now;
                    result.views_average = result.views_total * result.views_average / (result.views_total - result.views_today + result.views_average);
                    result.views_total += 1;
                    result.views_today = 1;
                }

                AnalyticInfoStorage.Update(_context.BaseUri.OriginalString, result);

                //_context.UpdateObject(result);
                //_context.SaveChanges();
            }
        }

        public void RegisterDownload(string itemKey)
        {
            AnalyticInfoStorage.AddDownloadAnalytic(_context.BaseUri.OriginalString, itemKey);

        }

        public void IncrementVote(string itemKey, int vote)
        {
            lock (Mutex) // NOTE: This synchronization is not enough since there may be several instances of roles.
            {
                var result = GetOrCreateAnalyticInfo(itemKey);

                if (vote < 0)
                    result.NegativeVotes += -vote;
                else
                    result.PositiveVotes += vote;

                AnalyticInfoStorage.Update(_context.BaseUri.OriginalString, result);
                //_context.UpdateObject(result);
                //_context.SaveChanges();
            }
        }

        public IEnumerable<AnalyticInfo> SelectAll()
        {
            return from dsi in _context.AnalyticInfo select dsi;
        }

        public AnalyticInfo GetAnalyticSummary(string itemKey)
        {
            return GetOrCreateAnalyticInfo(itemKey);
        }

        private AnalyticInfo GetOrCreateAnalyticInfo(string itemKey)
        {
            AnalyticInfo dataset = AnalyticInfoStorage.GetAnalyticInfo(_context.BaseUri.OriginalString, itemKey);

            if (dataset != null)
                return dataset;

            var initialViewCount = new Random().Next(3, 12); // This is to attract users.
            dataset = new AnalyticInfo(itemKey)
                        {
                            last_viewed = DateTime.Now,
                            views_today = initialViewCount,
                            views_total = initialViewCount,
                            views_average = initialViewCount,
                            NegativeVotes = 0,
                            PositiveVotes = 0,
                        };

            AnalyticInfoStorage.AddAnalyticInfo(_context.BaseUri.OriginalString, itemKey);
            return dataset;
        }
    }
}

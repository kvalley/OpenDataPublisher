using System;
using System.Net;
using System.Web;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

using Odp.Data;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.Data.DataSets
{
    public sealed class DatasetInfoDataContext : sqlTableServiceContext
    {
        public DatasetInfoDataContext(string baseAddress)
            : base(baseAddress)
        {
        }

        public IQueryable<AnalyticInfo> AnalyticInfo
        {
            get
            {
                return AnalyticInfoStorage.GetAllAnalyticInfos(BaseUri.OriginalString);
            }
        }
    }
}

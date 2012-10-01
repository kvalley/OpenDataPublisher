using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;

namespace Odp.Data.Views
{
    public class ViewDataSourceContext : sqlTableServiceContext
    {
        public static readonly string ViewTableName = "ViewEntries";

        public ViewDataSourceContext(string baseAddress)
            : base( baseAddress)
        {
        }

        public IQueryable<ViewEntry> ViewEntries 
        {
            get
            {
                return CreateQuery<ViewEntry>(ViewTableName);
            }
        }
    }
}

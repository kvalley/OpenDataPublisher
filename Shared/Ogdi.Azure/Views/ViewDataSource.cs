using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;

namespace Odp.Data.Views
{
    public class ViewDataSource
    {
        private static readonly object Mutex = new object();

        private readonly ViewDataSourceContext _context; 

        static ViewDataSource()
        {
            sqlTableClient.CreateTablesFromModel(typeof(ViewDataSourceContext), System.Configuration.ConfigurationManager.AppSettings["serviceUri"]);
        }

        public ViewDataSource()
        {
            _context = new ViewDataSourceContext(System.Configuration.ConfigurationManager.AppSettings["serviceUri"]);
        } 

        public void AddView(ViewEntry item)
        {
            this._context.AddObject(ViewDataSourceContext.ViewTableName, item);
            this._context.SaveChanges();
        }

        public IEnumerable<ViewEntry> SelectAll()
        {
            return this._context.ViewEntries.AsEnumerable();
        }
    }
}

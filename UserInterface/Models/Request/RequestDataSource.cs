using System;
using System.Collections.Generic;
using System.Linq;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.UserInterface.Models.Request
{
    public class RequestDataSource
    {
        static RequestDataSource()
        {
        }

        public RequestDataSource()
        {
        }

        public void AddRequest(RequestEntry item)
        {
        }

        public void UpdateRequest(RequestEntry item)
        {
        }

        public void DeleteRequest(string requestId)
        {
        }

        public IEnumerable<RequestEntry> Select()
        {
            var list = new List<RequestEntry>();
            return list.AsEnumerable<RequestEntry>();
        }

        public RequestEntry GetById(string requestId)
        {
            return null;
        }

        public IQueryable<RequestEntry> SelectAll()
        {
            var list = new List<RequestEntry>();
            return list.AsQueryable<RequestEntry>();
        }

        public IQueryable<RequestEntry> SelectAllWithHidden()
        {
            var list = new List<RequestEntry>();
            return list.AsQueryable<RequestEntry>();
        }
    }
}

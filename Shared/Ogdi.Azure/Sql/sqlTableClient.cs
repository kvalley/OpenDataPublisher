using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Views;

namespace Odp.Data.Sql
{
    public class sqlTableClient
    {
        public sqlTableClient(string baseAddress)
        {
        }

        public Uri BaseUri { get; set; }

        public TimeSpan Timeout { get; set; }

        public IAsyncResult BeginCreateTable(string tableName, AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginCreateTableIfNotExist(string tableName, AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginDeleteTable(string tableName, AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginDeleteTableIfExist(string tableName, AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginDoesTableExist(string tableName, AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginListTablesSegmented(AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginListTablesSegmented(string prefix, AsyncCallback callback, object state)
        {
            return null;
        }
        public IAsyncResult BeginListTablesSegmented(string prefix, int maxResults, sqlResultContinuation continuationToken, AsyncCallback callback, object state)
        {
            return null;
        }
        public void CreateTable(string tableName)
        {
        }
        public bool CreateTableIfNotExist(string tableName)
        {
            return false;
        }
        public static void CreateTablesFromModel(Type serviceContextType, string baseAddress)//, StorageCredentials credentials)
        {
        }
        public void DeleteTable(string tableName)
        {
        }
        public bool DeleteTableIfExist(string tableName)
        {
            return false;
        }
        public bool DoesTableExist(string tableName)
        {
            return false;
        }
        public void EndCreateTable(IAsyncResult asyncResult)
        {
        }
        public bool EndCreateTableIfNotExist(IAsyncResult asyncResult)
        {
            return false;
        }
        public void EndDeleteTable(IAsyncResult asyncResult)
        {
        }
        public bool EndDeleteTableIfExist(IAsyncResult asyncResult)
        {
            return false;
        }
        public bool EndDoesTableExist(IAsyncResult asyncResult)
        {
            return false;
        }
        public List<string> EndListTablesSegmented(IAsyncResult asyncResult)
        {
            return null;
        }
        public sqlTableServiceContext GetDataServiceContext()
        {
            return null;
        }
        public IEnumerable<string> ListTables()
        {
            return null;
        }
        public IEnumerable<string> ListTables(string prefix)
        {
            return null;
        }
        public List<string> ListTablesSegmented()
        {
            return null;
        }
        public List<string> ListTablesSegmented(int maxResults)//, ResultContinuation continuationToken)
        {
            return null;
        }
        public List<string> ListTablesSegmented(string prefix, int maxResults)//, ResultContinuation continuationToken)
        {
            return null;
        }
    }
}

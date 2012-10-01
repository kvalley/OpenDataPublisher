using System;
using System.Xml.Linq;
using System.IO;
using System.Xml;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;
using Odp.InteractiveSdk.Mvc.Models;

namespace Odp.InteractiveSdk.Mvc.Repository
{

    internal class BlobRepositary
    {
        static BlobRepositary()
        {
        }

        internal static string GetBlobUrl(string containerName, string tableName, string ext)
        {
            throw new Exception("We don't support prepared binary files.");
        }

        internal static sqlBlockBlob CreateBlob(string containerName, string tableName, string ext)
        {
            return null;
        }
    }
}
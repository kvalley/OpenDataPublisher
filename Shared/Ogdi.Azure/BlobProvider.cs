//
// <copyright file="BlobProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Microsoft.Samples.ServiceHosting.AspProviders
{

    public enum EventKind
    {
        Critical,
        Error,
        Warning,
        Information,
        Verbose
    };

    static class Log
    {
        internal static void Write(EventKind eventKind, string message, params object[] args)
        {
            switch (eventKind)
            {
                case EventKind.Error:
                case EventKind.Critical:
                    Trace.TraceError(message, args);
                    break;
                case EventKind.Warning:
                    Trace.TraceWarning(message, args);
                    break;
                case EventKind.Information:
                case EventKind.Verbose:
                    Trace.TraceInformation(message, args);
                    break;
            }
        }
    }

    internal class BlobProvider
    {
        internal BlobProvider(Uri baseUri, string containerName)
        {
        }

        internal string ContainerUrl
        {
            get
            {
                return "ContainerUrl";
            }
        }

        internal bool GetBlobContentsWithoutInitialization(string blobName, Stream outputStream, out sqlBlobProperties properties)
        {
            properties = new sqlBlobProperties();
            return false;

        }

        internal MemoryStream GetBlobContent(string blobName, out sqlBlobProperties properties)
        {
            properties = null;
            return null;
        }


        internal sqlBlobProperties GetBlobContent(string blobName, Stream outputStream)
        {
            return null;
        }

        internal void UploadStream(string blobName, Stream output)
        {
            //UploadStream(blobName, output, true);
        }

        internal bool UploadStream(string blobName, Stream output, bool overwrite)
        {
            return false;
        }

        internal bool DeleteBlob(string blobName)
        {
            return false;
        }

        internal bool DeleteBlobsWithPrefix(string prefix)
        {
            return false;
        }

        public IEnumerable<sqlListBlobItem> ListBlobs(string folder)
        {
            return null;
        }

        private sqlBlobContainer GetContainer()
        {
            return null;
        }

    }
}

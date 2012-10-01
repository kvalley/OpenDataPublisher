using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Odp.Data.Sql;

namespace InteractiveSdk.WorkerRole.MessageHandlers
{
    /// <summary>
    /// Converts dataset to several formats to storage blobs
    /// </summary>
    internal class ConvertDataHandler : IMessageHandler
    {

        internal ConvertDataHandler(sqlBlobContainer container)
        {
        }

        public void ProcessMessage(string msg)
        {            
        }
    }
}

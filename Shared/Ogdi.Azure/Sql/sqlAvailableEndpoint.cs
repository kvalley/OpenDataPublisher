using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Odp.Data.Sql
{
    public class sqlAvailableEndpoint
    {
        public sqlAvailableEndpoint()
        {
        }

        // Properties are all lowercase in the Azure table because this was the 
        // naming convention we picked for consistency due to varying data source.
        public string alias { get; set; }
        public string description { get; set; }
        public string disclaimer { get; set; }
        public string storageaccountname { get; set; }
        public string storageaccountkey { get; set; }
    }
}

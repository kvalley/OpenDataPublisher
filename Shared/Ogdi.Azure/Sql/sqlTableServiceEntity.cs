using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Views;

namespace Odp.Data.Sql
{
	// Summary:
	//     Represents an entity in a table.
	//[CLSCompliant(false)]
	public abstract class sqlTableServiceEntity
	{		
        public virtual string PartitionKey { get; set; }
		public virtual string RowKey { get; set; }
		public DateTime Timestamp { get; set; }

		protected sqlTableServiceEntity()
		{
			PartitionKey = "";
			RowKey = "";
		}

		protected sqlTableServiceEntity(string partitionKey, string rowKey)
		{
			PartitionKey = partitionKey;
			RowKey = rowKey;
		}
	}
}

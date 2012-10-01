using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.Data.Sql
{
	public class sqlPropertyMetadata
	{
		public string name;
		public string dataType;
		public string nullable;
	}

	public class sqlTableMetadata
	{
		public string name;
		public List<sqlPropertyMetadata> properties;
	}
}

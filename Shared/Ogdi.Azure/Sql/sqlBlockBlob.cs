using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Views;

namespace Odp.Data.Sql
{
	public class sqlBlockBlob
	{
	}

	public class sqlBlobProperties {
		public sqlBlobProperties()
		{
		}

		public sqlBlobProperties(sqlBlobProperties other)
		{
		  BlobType = other.BlobType;
		  CacheControl = other.CacheControl;
		  ContentEncoding = other.ContentEncoding;
		  ContentLanguage = other.ContentLanguage;
		  ContentMD5 = other.ContentMD5;
		  ContentType = other.ContentType;
		  ETag = other.ETag;
		  LastModifiedUtc = other.LastModifiedUtc;
		  LeaseStatus = other.LeaseStatus;
		  Length = other.Length;
		}

		public int BlobType { get; internal set; }
		public string CacheControl { get; set; }
		public string ContentEncoding { get; set; }
		public string ContentLanguage { get; set; }
		public string ContentMD5 { get; set; }
		public string ContentType { get; set; }
		public string ETag { get; internal set; }
		public DateTime LastModifiedUtc { get; internal set; }
		public int LeaseStatus { get; internal set; }
		public long Length { get; internal set; }
	}
	
	public class sqlBlobContainer
	{
	}
	
	public class sqlListBlobItem
	{
		public Uri Uri;// { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Services.Client;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Views;

namespace Odp.Data.Sql
{
	public class sqlTableServiceContext : DataServiceContext
	{
	  public sqlTableServiceContext(string baseAddress):base(new Uri(baseAddress))
	  {
	  }
	  public IAsyncResult BeginSaveChangesWithRetries(AsyncCallback callback, object state)
	  {
			return null;
	  }
	  public IAsyncResult BeginSaveChangesWithRetries(SaveChangesOptions options, AsyncCallback callback, object state)
	  {
			return null;
	  }
	  public DataServiceResponse EndSaveChangesWithRetries(IAsyncResult asyncResult)
	  {
			return null;
	  }
	  public DataServiceResponse SaveChangesWithRetries()
	  {
			return null;
	  }
	  public DataServiceResponse SaveChangesWithRetries(SaveChangesOptions options)
	  {
			return null;
	  }
	}
}

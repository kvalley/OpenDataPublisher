using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Odp.Data;
using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.UserInterface.Models.Comments
{
    public class CommentsDataContext : sqlTableServiceContext
    {
        //public CommentsDataContext(string baseAddress, StorageCredentials credentials)
        public CommentsDataContext(string baseAddress)
            : base(baseAddress)
        {
        }

        public IQueryable<CommentEntry> Comments
        {
            get
            {
                //TODO when all this wil be configured in some way, until then we can't do anything
                return null;
            }
        }
    }
}

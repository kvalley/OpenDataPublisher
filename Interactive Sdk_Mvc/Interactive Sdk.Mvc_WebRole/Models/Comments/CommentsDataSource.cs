using System;
using System.Collections.Generic;
using System.Linq;

using Odp.InteractiveSdk.Mvc.Models.Request;
using Odp.InteractiveSdk.Mvc.Repository;

namespace Odp.InteractiveSdk.Mvc.Models.Comments
{
    public class CommentsDataSource
    {
        static CommentsDataSource()
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
        }

        public CommentsDataSource()
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
            //this.context = new CommentsDataContext(account.TableEndpoint.AbsoluteUri, account.Credentials);
            //this.context.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1));
        }

        public void AddComment(CommentEntry item)
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
            //this.context.AddObject("Comments", item);
            //this.context.SaveChanges();
        }

        public void DeleteComment(string id)
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
        }

        public void DeleteByParent(string parentId, string container)
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
        }

        public void UpdateStatusByParent(string parentId, string container, string status)
        {
        }

        public IEnumerable<CommentEntry> SelectAll()
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
            var list = new List<CommentEntry>();
            return list.AsEnumerable<CommentEntry>();
        }

        public IEnumerable<CommentEntry> SelectAllWithHidden()
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
            var list = new List<CommentEntry>();
            return list.AsEnumerable<CommentEntry>();
        }

        public CommentEntry GetById(string id)
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
            return new CommentEntry()
            {
                Subject = "Subject",
                Comment = "Comment",
                Username = "Username",
                PostedOn = DateTime.Now,
                Email = "Email",
                Type = "Type",
                Status = "Status",
                Notify = false
            };
        }

        public void Update(CommentEntry entry)
        {
            //TODO when all this wil be configured in some way, until then we can't do anything
        }
    }
}

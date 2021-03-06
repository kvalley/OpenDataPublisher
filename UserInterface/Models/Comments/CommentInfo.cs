﻿using System;
using System.Collections.Generic;
using Odp.UserInterface.Repository;

namespace Odp.UserInterface.Models.Comments
{
	public class CommentInfo
	{
		public string DatasetId { get; set; }
		public string DatasetName { get; set; }
		public List<Comment> Comments { get; set; }
		public List<String> CommentTypes { get; set; }
		public ParentType ParentType { get; set; }
		public string Container { get; set; }

		protected CommentInfo()
		{
			Comments = new List<Comment>();
			CommentTypes = new List<String>();
			PopulateCommentTypes();
		}

		public CommentInfo(string datasetId) : this()
		{
			DatasetId = datasetId;
            LoadComments();
		}

        public CommentInfo(string datasetId, ParentType parentType)
            : this()
        {
            DatasetId = datasetId;
            ParentType = parentType;
            LoadComments();
        }

        public static CommentInfo CreateDatasetCommentInfo(string container, string entityName, string datasetName)
        {
            CommentInfo result = new CommentInfo();
			result.DatasetName = datasetName;
            result.Container = container;
            result.DatasetId = entityName;
            result.ParentType = ParentType.Dataset;
            result.LoadComments();
            return result;
        }

        public static CommentInfo CreateRequestCommentInfo(string requestId)
        {
            CommentInfo result = new CommentInfo();
            result.Container = "Request";
            result.DatasetId = requestId;
            result.ParentType = ParentType.Request;
            result.LoadComments();
            return result;
        }

		private void PopulateCommentTypes()
		{
			CommentTypes.Add("General Comment (no reply required)");
            CommentTypes.Add("General Comment (reply required)");
			CommentTypes.Add("Data Request");
			CommentTypes.Add("Data Error");
		}

        private void LoadComments()
        {
            CommentsDataSource ds = new CommentsDataSource();
            Comments = new List<Comment>(CommentRepository.GetDatasetComments(DatasetId, ParentType.ToString(), Container));
        }
	}
}

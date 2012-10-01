using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.InteractiveSdk.Mvc.Models
{
    public class DataLoaderModel
    {
        string save;

        public string Save
        {
            get { return save; }
            set { save = value; }
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string table;

        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        string dataSource;

        public string DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        string category;

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        bool newRecord = false;

        public bool NewRecord
        {
            get { return newRecord; }
            set { newRecord = value; }
        }

        HttpPostedFileBase file;

        public HttpPostedFileBase File
        {
            get { return file; }
            set { file = value; }
        }

        bool uploadOnly = true;

        public bool UploadOnly
        {
            get { return uploadOnly; }
            set { uploadOnly = value; }
        }

        string fileLocation;

        public string FileLocation
        {
            get { return fileLocation; }
            set { fileLocation = value; }
        }

        string fileExtention;

        public string FileExtention
        {
            get { return fileExtention; }
            set { fileExtention = value; }
        }

        bool hasHeaders = true;

        public bool HasHeaders
        {
            get { return hasHeaders; }
            set { hasHeaders = value; }
        }

        string delimiter;

        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

        object dataSet;

        public object DataSet
        {
            get { return dataSet; }
            set { dataSet = value; }
        }

        string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        string container;

        public string Container
        {
            get { return container; }
            set { container = value; }
        }

        string entitySetName;

        public string EntitySetName
        {
            get { return entitySetName; }
            set { entitySetName = value; }
        }

        private DatasetListModel otherModel;

        public DatasetListModel OtherModel
        {
            get { return otherModel; }
            set { otherModel = value; }
        }

        private DataBrowserModel viewDataModel;

        public DataBrowserModel ViewDataModel
        {
            get { return viewDataModel; }
            set { viewDataModel = value; }
        }
    }
}
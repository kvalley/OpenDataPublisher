using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.InteractiveSdk.Mvc.Models
{
    public class CategoryModel
    {
        private string save;

        public string Save
        {
            get { return save; }
            set { save = value; }
        }

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        private DatasetListModel otherModel;

        public DatasetListModel OtherModel
        {
            get { return otherModel; }
            set { otherModel = value; }
        }

    }
}
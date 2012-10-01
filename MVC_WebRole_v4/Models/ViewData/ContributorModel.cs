using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Odp.InteractiveSdk.Mvc.Models
{
    public class ContributorModel
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

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string disclaimer;

        public string Disclaimer
        {
            get { return disclaimer; }
            set { disclaimer = value; }
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
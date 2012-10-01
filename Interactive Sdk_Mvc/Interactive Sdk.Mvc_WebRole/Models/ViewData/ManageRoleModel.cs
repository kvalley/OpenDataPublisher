using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Odp.InteractiveSdk.Mvc.Models
{
    [Bind(Exclude = "RoleNames")]
    public class ManageRolesModel
    {
        public string UserName { get; set; }
        public SelectList RoleNames { get; set; }

        public ManageRolesModel()
        {
            RoleNames = new SelectList(Roles.GetAllRoles(), "Public");
        }

        public ManageRolesModel(string userName)
            : this()
        {
            UserName = userName;
        }
    }
}

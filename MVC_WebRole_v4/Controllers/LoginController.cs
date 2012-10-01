using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Security;

namespace Odp.InteractiveSdk.Mvc.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string login, string userName, string password, bool persistent)
        {
            if (login != "Cancel")
            {
                try
                {
                    string strUserInputtedHashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");

                    if (strUserInputtedHashedPassword == getHashPassword(userName))
                    {
                        //Create the ticket, and add the groups. 
                        bool isCookiePersistent = persistent;
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(60), isCookiePersistent, "");

                        //Encrypt the ticket. 
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        //Create a cookie, and then add the encrypted ticket to the cookie as data. 
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                        if ((isCookiePersistent == true))
                        {
                            authCookie.Expires = authTicket.Expiration;
                        }
                        //Add the cookie to the outgoing cookies collection. 
                        Response.Cookies.Add(authCookie);

                        // Done
                        return RedirectToAction("DataSetList", "DataCatalog");
                    }
                    else
                    {
                        //lblError.Text = "Authentication did not succeed. Check user name and password.";
                    }
                }

                catch (Exception ex)
                {
                    throw (ex);
                    //Odp.Data.ErrorLog.WriteError(ex.Message);
                    return PartialView("ErrorView", ViewData.Model);
                }
            }
            else
            {
                // Done
                return RedirectToAction("DataSetList", "DataCatalog");
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        private string getHashPassword(string userName)
        {
            var sql = Odp.Data.Sql.sqlServerConnection.GetDataServiceInstance();
            return sql.getHashPassword(userName);
        }
    }
}

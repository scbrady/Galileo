using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Galileo;
using Galileo.Database;
using Galileo.Models;

namespace Galileo
{
    public static class GlobalVariables
    {
        // Set the current user when the web app is first hit
        public static User CurrentUser
        {
            get
            {
                // Get the ID of the currently logged in user
                if (HttpContext.Current.Session["username"] == null)
                {
                    //string loginId = HttpContext.Current.User.Identity.Name;
                    //string userId = loginId.Substring(loginId.LastIndexOf('\\') + 1);
                    //HttpContext.Current.Session["username"] = userId;
                    HttpContext.Current.Session["username"] = "mgeary";
                }

                // Fetch the user from the Time Machine database
                DatabaseRepository db = new DatabaseRepository();
                return db.GetUser((string)HttpContext.Current.Session["username"]);
            }
        }
    }
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMappings();
        }
    }
}

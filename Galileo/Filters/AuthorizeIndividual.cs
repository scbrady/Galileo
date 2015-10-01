using Galileo.Database;
using Galileo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Galileo.Filters
{
    public class AuthorizeIndividual : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DatabaseRepository db = new DatabaseRepository();
            string userId = filterContext.ActionParameters["userId"].ToString();
            User currentUser = GlobalVariables.CurrentUser;
            List<User> users = db.GetMinions(currentUser.user_id, currentUser.user_is_teacher);

            // If the user is not the commenter, redirect
            if (!users.Any( u => u.user_id == userId))
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new { action = "Index", controller = "Error" }));
        }
    }
}
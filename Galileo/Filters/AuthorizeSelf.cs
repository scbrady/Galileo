using Galileo.Database;
using Galileo.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace Galileo.Filters
{
    public class AuthorizeSelf : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string leaderId = filterContext.ActionParameters["leaderId"].ToString();

            // If the user is not the commenter, redirect
            if (leaderId != GlobalVariables.CurrentUser.user_id)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new { action = "Index", controller = "Error" }));
        }
    }
}
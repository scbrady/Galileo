using Galileo.Database;
using Galileo.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace Galileo.Filters
{
    public class AuthorizeCommenter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DatabaseRepository db = new DatabaseRepository();
            int commentId = (int) filterContext.RouteData.Values["commentId"];
            Comment comment = db.GetComment(commentId);

            // If the user is not the commenter, redirect
            if (comment.commenter_id != GlobalVariables.CurrentUser.user_id)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new { action = "Index", controller = "Error" }));
        }
    }
}
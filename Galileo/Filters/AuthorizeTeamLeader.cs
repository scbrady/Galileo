using Galileo.Database;
using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Galileo.Filters
{
    public class AuthorizeTeamLeader : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DatabaseRepository db = new DatabaseRepository();
            int teamId = (int) filterContext.ActionParameters["teamId"];
            User currentUser = GlobalVariables.CurrentUser;
            List<Project> teams = db.GetLeaderProjects(currentUser.user_id);

            if (!teams.Any(t => t.project_id == teamId) && !currentUser.user_is_teacher)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new { action = "Index", controller = "Error" }));
        }
    }
}
﻿using System;
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
            var teamId = filterContext.ActionParameters["teamId"];

            if (!(bool)HttpContext.Current.Session["userIsTeacher"])
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new { action = "Index", controller = "Error" }));
        }
    }
}
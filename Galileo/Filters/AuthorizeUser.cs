using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Galileo.Filters
{
    public class AuthorizeUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                User currentUser = GlobalVariables.CurrentUser;
                string error = HttpContext.Current.Request.Path;

                HttpContext.Current.Session["userIsStudent"] = false;
                HttpContext.Current.Session["userIsTeacher"] = false;
                HttpContext.Current.Session["userIsManager"] = false;

                if (currentUser == null || !currentUser.user_is_enabled)
                {
                    if (!error.Contains("Error"))
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new { action = "Index", controller = "Error" }));
                }
                else
                {
                    HttpContext.Current.Session["userIsStudent"] = currentUser.user_is_student;
                    HttpContext.Current.Session["userIsTeacher"] = currentUser.user_is_teacher;
                    HttpContext.Current.Session["userIsManager"] = currentUser.user_is_manager;
                }


            }
            catch
            {
                HttpContext.Current.Response.Redirect("http://csmain/seproject/development/resources/500.htm");
            }
        }
    }
}
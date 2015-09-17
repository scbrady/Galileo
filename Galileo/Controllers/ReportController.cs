using Galileo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Galileo.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            // Check if this is a teacher, pm, team leader, or team member
            // Teachers gets this page with all the courses listed with a summary of hours for each course
            // PMs get redirected to project(?) page
            // Team Leaders get redirected to their team page
            // Team members get redirected to their individual page
            User user = GlobalVariables.CurrentUser;
            return View(user);
        }

        public ActionResult Course(int courseId)
        {
            // This view will show all of the projects in the course with a summary of hours for each project
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Project(int projectId)
        {
            // This view will show all the teams (or if no teams, individuals) with a summary of hours for each one
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Team(int teamId)
        {
            // This view will show all of the team members with a summary of hours for each one
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Individual(string userId)
        {
            // This view will show all of the info for an individual team member
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
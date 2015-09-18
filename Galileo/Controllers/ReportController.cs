using Galileo.Database;
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
            TimeMachineRepository db = new TimeMachineRepository();
            User user = GlobalVariables.CurrentUser;
            // Check if this is a teacher, pm, team leader, or team member
            // Teachers gets this page with all the courses listed with a summary of hours for each course
            // PMs get redirected to courses page that only shows them the projects they are over
            // Team Leaders get redirected to their team page
            // Team members get redirected to their individual page
            List<Course> courses = db.GetCourses(user.user_id);
            return View(courses);
        }

        public ActionResult Course(int courseId)
        {
            TimeMachineRepository db = new TimeMachineRepository();
            // This view will show all of the projects in the course with a summary of hours for each project
            List<Project> projects = db.GetProjects(courseId);

            return View(projects);
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
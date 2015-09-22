using Galileo.Database;
using Galileo.Models;
using Galileo.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace Galileo.Controllers
{
    public class TeamController : Controller
    {
        public ActionResult Index()
        {
            DatabaseRepository db = new DatabaseRepository();
            User user = GlobalVariables.CurrentUser;
            List<Course> courses = db.GetCourses(user.user_id);
            return View(courses);
        }

        public ActionResult Teams(int courseId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<Project> projects = db.GetProjects(courseId);
            List<User> members = db.GetUsersInCourse(courseId);

            var viewModel = new CourseProjectsAndUsers()
            {
                projects = projects,
                users = members
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseProjectsAndUsers test)
        {
            // This will add the course to the DB and redirect back to the course page
            DatabaseRepository db = new DatabaseRepository();

            int[] projectIds = test.teams.Where(t => t.userIds != null && t.projectId != 0).Select(t => t.projectId).ToArray();
            if(!string.IsNullOrEmpty(test.projectManager))
                db.InsertProjectManager(test.projectManager, projectIds);

            db.InsertTeamMembers(test.teams.ToList());

            return RedirectToAction("Index");
        }
    }
}
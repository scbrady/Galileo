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
        public ActionResult Create(CourseProjectsAndUsers course)
        {
            DatabaseRepository db = new DatabaseRepository();

            int[] projectIds = course.teams.Where(t => t.userIds != null && t.projectId != 0).Select(t => t.projectId).ToArray();
            if(!string.IsNullOrEmpty(course.projectManager))
                db.InsertProjectManager(course.projectManager, projectIds);

            db.InsertTeamMembers(course.teams.ToList());

            return RedirectToAction("Index");
        }
    }
}
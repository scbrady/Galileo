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
        /// <summary>
        /// If the user has Teacher privileges, this will list all courses the user teaches
        /// </summary>
        /// <returns>List of Courses</returns>
        public ActionResult Index()
        {
            DatabaseRepository db = new DatabaseRepository();
            User user = GlobalVariables.CurrentUser;
            List<Course> courses = db.GetCourses(user.user_id);
            return View(courses);
        }

        /// <summary>
        /// Lists all teams taking a given course
        /// </summary>
        /// <param name="courseId">ID variable of course in question</param>
        /// <returns>List of Teams</returns>
        public ActionResult Teams(int courseId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<Project> projects = db.GetProjects(courseId);
            List<User> members = db.GetUsersInCourse(courseId);

            var viewModel = new TeamsProjectsAndUsers()
            {
                projects = projects,
                users = members
            };
            return View(viewModel);
        }

        /// <summary>
        /// Checks and validates identity of user, then redirects them to the Index with proper permissions
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Sends user to Index page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamsProjectsAndUsers course)
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
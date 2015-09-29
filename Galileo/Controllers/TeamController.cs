using Galileo.Database;
using Galileo.Models;
using Galileo.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using AutoMapper;

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
            List<Module> viewModel = Mapper.Map<List<Course>, List<Module>>(courses);
            return View(viewModel);
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
            List<User> users = db.GetUsersWithoutTeams(courseId);
            List<User> members = db.GetUsersWithTeams(courseId);

            var viewModel = new TeamsProjectsAndUsers()
            {
                projects = projects,
                users = users,
                members = members
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
            int[] allProjectIds = course.teams.Where(t => t.projectId != 0).Select(t => t.projectId).ToArray();
            int[] populatedProjectIds = course.teams.Where(t => t.userIds != null && t.projectId != 0).Select(t => t.projectId).ToArray();

            if (allProjectIds.Any())
            {
                db.DeleteAllMembers(allProjectIds);

                if (!string.IsNullOrEmpty(course.projectManager))
                {
                    string projectManagerId = course.projectManager.Split(',').First();
                    db.InsertProjectManager(projectManagerId, populatedProjectIds);
                }

                db.InsertTeamMembers(course.teams.ToList());
            }
            return RedirectToAction("Index");
        }
    }
}
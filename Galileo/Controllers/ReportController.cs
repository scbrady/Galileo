using AutoMapper;
using Galileo.Database;
using Galileo.Filters;
using Galileo.Models;
using Galileo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Galileo.Controllers
{
    public class ReportController : Controller
    {
        /// <summary>
        /// Checks if the logged in user is a teacher, project manager, team leader, or team member and redirects accordingly
        /// Teachers get sent to the page that lists every course they have access to
        /// Project Managers get sent to a page that shows what projects they are managing
        /// Team Leaders get sent to their team page
        /// Team Members get sent to their individual page
        /// </summary>
        public ActionResult Index()
        {
            User user = GlobalVariables.CurrentUser;

            if (user.user_is_teacher)
                return RedirectToAction("Courses");
            else
                return RedirectToAction("Individual", new { userId = user.user_id });
        }

        /// <summary>
        /// Lists all of the courses that a teacher has access to
        /// Gives a summary of the hours spent in each course
        /// </summary>
        [AuthorizeTeacher]
        public ActionResult Courses()
        {
            DatabaseRepository db = new DatabaseRepository();
            User user = GlobalVariables.CurrentUser;
            List<Course> courses = db.GetCourses(user.user_id);
            List<Module> viewModel = Mapper.Map<List<Course>, List<Module>>(courses);
            return View(viewModel);
        }

        /// <summary>
        /// Lists all of the projects, teams, and users inside a course
        /// Gives a summary of the hours spent in each project/team
        /// </summary>
        /// <param name="courseId">The ID of the course that the projects are in</param>
        [AuthorizeTeacher]
        public ActionResult Projects(int courseId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<Project> projects = db.GetProjects(courseId);
            List<User> members = db.GetUsersInCourse(courseId);

            var viewModel = new CourseProjectsAndUsers()
            {
                projects = Mapper.Map<List<Project>, List<Module>>(projects.Where(p => p.project_is_team == false).ToList()),
                teams = Mapper.Map<List<Project>, List<Module>>(projects.Where(p => p.project_is_team == true).ToList()),
                users = Mapper.Map<List<User>, List<Module>>(members)
            };
            return View(viewModel);
        }

        /// <summary>
        /// Lists all of the students that have logged time for a particular project
        /// Gives a summary of all of the hours the students have logged for that project
        /// </summary>
        /// <param name="projectId">The ID of the project to get the individuals for</param>
        /// <returns></returns>
        [AuthorizeTeacher]
        public ActionResult Project(int projectId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<User> members = db.GetUsersInProject(projectId);
            List<Module> viewModel = Mapper.Map<List<User>, List<Module>>(members);
            return View(viewModel);
        }

        /// <summary>
        /// Lists all of the students that are in a particular team
        /// Gives a summary of all of the hours each team member has logged for that team
        /// </summary>
        /// <param name="teamId">The ID of the team to get the members for</param>
        /// <returns></returns>
        public ActionResult Team(int teamId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<User> members = db.GetUsersInTeam(teamId);

            TeamMembers teamMembers = new TeamMembers
            {
                projectManager = Mapper.Map<List<User>, List<Module>>(members.Where(u => u.user_is_project_manager == true).ToList()),
                teamLeader = Mapper.Map<List<User>, List<Module>>(members.Where(u => u.user_is_team_leader == true).ToList()),
                teamMembers = Mapper.Map<List<User>, List<Module>>(members.Where(u => u.user_is_project_manager == false && u.user_is_team_leader == false).ToList())
            };

            return View(teamMembers);
        }

        /// <summary>
        /// Gives a detailed view of the individual
        /// Includes a summary of their hours as well as each time entry
        /// </summary>
        /// <param name="userId">The ID of the user to get the information for</param>
        /// <returns></returns>
        public ActionResult Individual(string userId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<Entry> entries = db.GetUserEntries(userId);
            return View(entries);
        }

        /// <summary>
        /// Lists all of the projects, teams, and users inside a course
        /// Gives a summary of the hours spent in each project/team
        /// </summary>
        /// <param name="courseId">The ID of the course that the projects are in</param>
        public JsonResult Entries(int courseId)
        {
            DatabaseRepository db = new DatabaseRepository();
            List<Entry> entries = db.GetEntriesForCourse(courseId);
            return Json(entries, JsonRequestBehavior.AllowGet);
        }
    }
}
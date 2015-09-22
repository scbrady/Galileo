using Galileo.Database;
using Galileo.Models;
using Galileo.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

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
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            // This will edit add or remove students from the teams that have been created
            return RedirectToAction("Course");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            // This will delete the team from the course
            return RedirectToAction("Course");
        }
    }
}
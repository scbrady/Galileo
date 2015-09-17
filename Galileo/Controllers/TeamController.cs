using Galileo.Models;
using System.Web.Mvc;

namespace Galileo.Controllers
{
    public class TeamController : Controller
    {
        public ActionResult Index()
        {
            // Show all courses that teams can be created for
            return View();
        }

        public ActionResult Course(int courseId)
        {
            // Show any teams in the course or offer to create some from the projects in the course
            // Show all users in the course so they can be assigned to the teams
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // This will add the course to the DB and redirect back to the course page
            return RedirectToAction("Course");
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
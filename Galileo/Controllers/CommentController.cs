using Galileo.Models;
using System.Web.Mvc;

namespace Galileo.Controllers
{
    public class CommentController : Controller
    {
        public ActionResult Index()
        {
            // Show all comments stored for the particular user
            // Should show all comments received and given
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            // This will add the comment to the DB and redirect back to the comment page
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            // This will edit the comment in the DB and redirect back to the comment page
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            // This will delete the comment from the DB and redirect back to the comment page
            return RedirectToAction("Index");
        }
    }
}
﻿using Galileo.Models;
using Galileo.ViewModels;
﻿using Galileo.Database;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Galileo.Controllers
{
    public class CommentController : Controller
    {
        public ActionResult Index()
        {
            User user = GlobalVariables.CurrentUser;
            // Show all comments stored for the particular user
            // Should show all comments received and given
            var comments = new Comments();
            comments.commenter_id = user.user_id;
            return View(comments);
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

        public JsonResult Users()
        {
            DatabaseRepository db = new DatabaseRepository();
            List<User> users = db.GetAllUsers();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}
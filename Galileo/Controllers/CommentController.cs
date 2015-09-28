using Galileo.Models;
using Galileo.ViewModels;
using Galileo.Database;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace Galileo.Controllers
{
    public class CommentController : Controller
    {
        public ActionResult Index()
        {
            User user = GlobalVariables.CurrentUser;
            DatabaseRepository db = new DatabaseRepository();
            // Show all comments stored for the particular user
            // Should show all comments received and given
            List<Comment> comments = db.GetComments(user.user_id);
            var groupedComments = comments.GroupBy(c => c.id).Select(c => new Comment
            {
                recipients = string.Join(", ", c.Select(comment => comment.recipient_first_name + ' ' + comment.recipient_last_name)),
                commenter = c.Select(comment => comment.commenter_first_name + ' ' + comment.commenter_last_name).First(),
                comment_text = c.Select(comment => comment.comment_text).First(),
                user_is_commenter = c.Select(comment => comment.user_is_commenter).First(),
                created_at = c.Select(comment => comment.created_at).First()
            }).ToList();

            var commentViewModel = new Comments
            {
                comments = groupedComments,
                commenter_id = user.user_id
            };

            return View(commentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comments newComment)
        {
            // This will add the comment to the DB and redirect back to the comment page
            DatabaseRepository db = new DatabaseRepository();

            if(!string.IsNullOrEmpty(newComment.comment))
            {
                int commentId = db.CreateComment(newComment.commenter_id, newComment.comment, newComment.hidden);
                db.LinkComment(commentId, newComment.recipients.Split(','));
            }

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
using Galileo.Models;
using Galileo.ViewModels;
using Galileo.Database;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Galileo.Filters;

namespace Galileo.Controllers
{
    public class CommentController : Controller
    {
        public ActionResult Index()
        {
            User user = GlobalVariables.CurrentUser;
            DatabaseRepository db = new DatabaseRepository();
            List<Comment> comments = db.GetComments(user.user_id);

            // Group comments and comma separate the recipients
            var groupedComments = comments.GroupBy(c => c.id).Select(c => new Comment
            {
                recipients = string.Join(", ", c.Select(comment => comment.recipient_first_name + ' ' + comment.recipient_last_name)),
                commenter = c.Select(comment => comment.commenter_first_name + ' ' + comment.commenter_last_name).First(),
                comment_text = c.Select(comment => comment.comment_text).First(),
                user_is_commenter = c.Select(comment => comment.user_is_commenter).First(),
                created_at = c.Select(comment => comment.created_at).First(),
                hidden = c.Select(comment => comment.hidden).First(),
                id = c.Select(comment => comment.id).First()
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

            if(!string.IsNullOrEmpty(newComment.comment) && !string.IsNullOrEmpty(newComment.recipients))
            {
                int commentId = db.CreateComment(newComment.commenter_id, newComment.comment, newComment.hidden);
                db.LinkComment(commentId, newComment.recipients.Split(','));
            }

            return RedirectToAction("Index");
        }

        [AuthorizeCommenter]
        public ActionResult Delete(int commentId)
        {
            // This will delete the comment from the DB and redirect back to the comment page
            DatabaseRepository db = new DatabaseRepository();

            db.DeleteComment(commentId);

            return RedirectToAction("Index");
        }

        public JsonResult Users()
        {
            User currentUser = GlobalVariables.CurrentUser;
            DatabaseRepository db = new DatabaseRepository();
            List<User> users = db.GetMinions(currentUser.user_id, currentUser.user_is_teacher);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}
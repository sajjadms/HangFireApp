using Hangfire;
using HangFireApp.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace HangFireApp.Controllers
{
    public class MailController : Controller
    {
        public static IList<NewCommentEmail> Emails = new List<NewCommentEmail>();

        public ActionResult CreateEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateEmail(Comment model)
        {
            if (ModelState.IsValid)
            {
                var email = new NewCommentEmail
                {
                    Id = Guid.NewGuid().ToString(),
                    To = model.To,
                    UserName = model.Username,
                    Comment = model.Text
                };

                Emails.Add(email);

                BackgroundJob.Enqueue(() => NotifyNewComment(email.Id));
            }

            return RedirectToAction("SendSuccess");
        }

        public ActionResult SendSuccess()
        {
            return View();       
        }

        [AutomaticRetry(Attempts = 20)] //The retry attempt count is limited (10 by default), but you can increase it
        public static void NotifyNewComment(string id)
        {
            // Prepare Postal classes to work outside of ASP.NET request
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            engines.Add(new FileSystemRazorViewEngine(viewsPath));

            var emailService = new EmailService(engines);

            var comment = Emails.FirstOrDefault(p=>p.Id == id);

            var email = new NewCommentEmail
            {
                To = comment.To,
                UserName = comment.UserName,
                Comment = comment.Comment
            };

            emailService.Send(email);

            //// Get comment and send a notification.
            //using (var db = new MailerDbContext())
            //{

            //}
        }
    }

   
}
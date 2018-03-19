using Microsoft.AspNet.Identity.Owin;
using Raven.Client.Documents.Session;
using System;
using System.Web;
using System.Web.Mvc;

namespace Sample.Controllers
{
    /// <summary>
    /// Controller that saves any changes in the Raven document session if the controller executed successfully.
    /// </summary>
    public abstract class RavenController : Controller
    {
        private IAsyncDocumentSession dbSession;

        public RavenController()
        {
        }

        /// <summary>
        /// Gets the Raven document session for this request.
        /// </summary>
        public IAsyncDocumentSession DbSession
        {
            get
            {
                if (this.dbSession == null)
                {
                    this.dbSession = HttpContext.GetOwinContext().Get<IAsyncDocumentSession>();
                }

                return dbSession;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                using (DbSession)
                {
                    if (filterContext.Exception == null && this.DbSession != null && this.DbSession.Advanced.HasChanges)
                    {
                        var saveTask = this.DbSession.SaveChangesAsync();

                        // Tell the task we don't need to invoke on MVC's SynchronizationContext. 
                        // Otherwise we can end up with deadlocks. See http://code.jonwagner.com/2012/09/04/deadlock-asyncawait-is-not-task-wait/
                        saveTask.ConfigureAwait(continueOnCapturedContext: false);
                        saveTask.Wait(TimeSpan.FromSeconds(10));
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZaKhan.BusinessLogic;
using ZaKhan.Model;

namespace ZaKhan.Controllers
{
    public class AuditController : Controller
    {
      public AuditBusiness _auditRecords = new AuditBusiness();

      public ActionResult AuditRecords(string SearchString)
       {
            ViewData["UName"] = new SelectList(_auditRecords.ViewAll(), "UserName", "UserName");

            if (!String.IsNullOrEmpty(SearchString))
            {
                return View(_auditRecords.Search(SearchString));
            }
            return View(_auditRecords.ViewAll());
       }
	}

    public class AuditAttribute : ActionFilterAttribute
    {
        public AuditBusiness _auditRecords = new AuditBusiness();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Stores the Request in an Accessible object
            var request = filterContext.HttpContext.Request;

            //Generate an audit
            AuditView audit = new AuditView()
            {
                AuditID = Guid.NewGuid(),
                Event = request.RawUrl,
                TimeAccessed = DateTime.UtcNow,
                UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
            };

            //Stores the Audit in the Database
            _auditRecords.Insert(audit);
            base.OnActionExecuting(filterContext);
        }
    }
}
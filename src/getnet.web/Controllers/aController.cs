using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using getnet.Helpers;
using getnet.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using System.Data.SqlClient;
using getnet.core.Model.Entities;

namespace getnet.Controllers
{
    [Authorize(Roles = Roles.GlobalViewers)]
    public class aController : BaseController
    {
        private Whistler logger = new Whistler(typeof(aController).FullName);

        [RedirectOnDbIssue]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [RedirectOnDbIssue]
        public IActionResult Search(string searchtext)
        {
            return View("Search", searchtext);
        }
        
        public IActionResult GetSiteStatus()
        {
            return Json(new
            {
                columns = uow.Repo<Site>().Get().GroupBy(d => d.Status).Select(d => new object[]{ d.Key.ToString(), d.Count() })
            });

        }

        [Route("/error")]
        public IActionResult Error()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception error = ex?.Error;
            if (error != null)
                logger.Error(error, WhistlerTypes.UnhandledException);
            
            if (HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("_error", error?.Message);
            }
            else
            {
                if (error?.GetType() == typeof(SqlException) || error?.InnerException?.GetType() == typeof(SqlException))
                {
                    Current.SetDbConfigurationState();
                    return RedirectToAction("index", "a");
                }
                else
                {
                    return View(error);
                }
            }
        }

        [AllowAnonymous]
        public IActionResult GetSnacks()
        {
            
            return Json(HttpContext.Session.GetSnackMessages().Concat(HttpContext.Request.GetRefererSnacks(RouteData)));
        }
        
        [AllowAnonymous]
        [Route("/dbissue")]
        public IActionResult Anon()
        {
            return View();
        }

        
        public IActionResult Alerts()
        {
            return View(uow.GetUserProfile(Current.User.AccountName).AlertRules);
        }

        public IActionResult AddAlert(AlertRule alert, string SiteName)
        {

            if (ModelState.IsValid)
            {
                if (SiteName != "Any")
                    alert.Site = uow.Repo<Site>().Get(d => d.Name == SiteName).FirstOrDefault();
                var chantes = uow.Repo<AlertRule>().Insert(alert);
                uow.Save();
                uow.GetUserProfile(Current.User.AccountName).AlertRules.AddOrNew(uow.Repo<AlertRule>().GetByID(chantes.CurrentValues["AlertRuleId"]));
                uow.Save();
                HttpContext.Session.AddSnackMessage("Alert added");
            }
            else
            {
                HttpContext.Session.AddSnackMessage(string.Join("; ", ModelState.Select(d => d.Value)));
            }
            return RedirectToAction("Alerts", uow.GetUserProfile(Current.User.AccountName).AlertRules);
        }

        public IActionResult DeleteAlert(int id)
        {
            var alert = uow.Repo<AlertRule>().Get(d => d.AlertRuleId == id, includeProperties: "User").FirstOrDefault();
            if (alert.User == uow.GetUserProfile(Current.User.AccountName))
            {
                uow.GetUserProfile(Current.User.AccountName).AlertRules.Remove(alert);
                uow.Repo<AlertRule>().Delete(id);
                uow.Save();
                HttpContext.Session.AddSnackMessage("Alert removed");
            }
            return RedirectToAction("Alerts", uow.GetUserProfile(Current.User.AccountName).AlertRules);
        }
    }
}

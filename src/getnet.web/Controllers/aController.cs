using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using getnet.Model;
using Microsoft.AspNetCore.Diagnostics;
using getnet.core;

namespace getnet.Controllers
{
    [RedirectOnDbIssue]
    [Authorize(Roles = Roles.GlobalAdmins)]
    public class aController : BaseController
    {
        private Whistler logger = new Whistler();

        public IActionResult Index()
        {
            return View();
        }

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
                return View(error);
            }
        }
    }
}

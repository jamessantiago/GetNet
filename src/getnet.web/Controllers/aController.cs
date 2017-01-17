﻿using System;
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
using System.Data.SqlClient;

namespace getnet.Controllers
{
    [RedirectOnDbIssue]
    public class aController : BaseController
    {
        private Whistler logger = new Whistler(typeof(aController).FullName);


        [Authorize]
        public IActionResult Index()
        {
            return View();
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
    }
}

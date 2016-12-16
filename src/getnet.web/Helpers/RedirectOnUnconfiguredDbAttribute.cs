using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using getnet.Controllers;
using getnet.core;

namespace getnet.Helpers
{
    public sealed class RedirectOnDbIssue : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (Current.ConfigurationRequired || Current.DatabaseConnectionError != null)
            {
                var controller = (BaseController)filterContext.Controller;
                filterContext.Result = controller.RedirectToAction("index", "init");
            }
        }
    }
}

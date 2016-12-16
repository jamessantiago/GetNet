using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace getnet.Controllers
{
    public class BaseController : Controller
    {
        public new RedirectToActionResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
    }
}

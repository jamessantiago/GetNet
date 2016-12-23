using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using getnet.Model;

namespace getnet.Controllers
{
    [RedirectOnDbIssue]
    [Authorize(Roles = Roles.GlobalAdmin)]
    public class aController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

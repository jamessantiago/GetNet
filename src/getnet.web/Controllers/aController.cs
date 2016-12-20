using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.Helpers;

namespace getnet.Controllers
{
    //[RedirectOnDbIssue]
    public class aController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

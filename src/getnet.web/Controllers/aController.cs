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
    [Authorize]
    public class aController : BaseController
    {
        public aController(UserManager<User> UserManager,
            SignInManager<User> SignInManager) : base(UserManager, SignInManager)
        {        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

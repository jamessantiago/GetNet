using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using getnet.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace getnet.Controllers
{
    public class AccountController : BaseController
    {
        protected UserManager<User> userManager;
        protected SignInManager<User> signInManager;

        public AccountController(UserManager<User> UserManager,
            SignInManager<User> SignInManager)
        {
            userManager = UserManager;
            signInManager = SignInManager;
        }
        
        [AllowAnonymous]
        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            email = email.ToLower();
            var tempUser = new User(email, uow.GetUserProfile(email));
            try
            {
                if (Retry.Do(() => getnet.Model.Security.LdapServer.Current.Authenticate(email, password), TimeSpan.FromSeconds(1), 2))
                {
                    await signInManager.SignInAsync(tempUser, true);
                    return RedirectToAction("index", "a");
                }
            } catch (AggregateException ex)
            {
                ViewData["LastError"] = ex.InnerExceptions.Last();
            }
            ViewData["FailedLoginMessage"] = "Username or password is incorrect";
            return View();
        }

        [Route("/logoff")]
        public async Task<IActionResult> Logoff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }
    }
}

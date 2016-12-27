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
using getnet.Model.Security;

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
        public async Task<IActionResult> Login()
        {
            if (Current.Security as EveryonesAnAdminProvider != null)
            {
                await signInManager.SignInAsync(new Model.User("AdminUser@GetNet", Roles.GlobalAdmins), false);
                return RedirectToAction("a", "index");
            }
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            email = email.ToLower();
            try
            {
                if (Retry.Do(() => getnet.Model.Security.LdapServer.Current.Authenticate(email, password), TimeSpan.FromSeconds(1), 2))
                {
                    var groups = (Current.Security as ActiveDirectoryProvider).GetRoles(email);
                    var tempUser = new User(email, groups.ToArray());
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

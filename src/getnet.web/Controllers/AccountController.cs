using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using getnet.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using getnet.Model.Security;
using Novell.Directory.Ldap;
using getnet.Helpers;

namespace getnet.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private Whistler logger = new Whistler(typeof(AccountController).FullName);

        protected UserManager<User> userManager;
        protected SignInManager<User> signInManager;

        public AccountController(UserManager<User> UserManager,
            SignInManager<User> SignInManager)
        {
            userManager = UserManager;
            signInManager = SignInManager;
        }
        
        [Route("/login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (Current.Security as EveryonesAnAdminProvider != null)
            {
                await signInManager.SignInAsync(new Model.User("AdminUser@GetNet", Roles.GlobalAdmins), false);
                if (returnUrl.HasValue())
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("index", "a");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [RequireHttps]
        [Route("/login")]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            username = username.ToLower();
            try
            {
                if (Retry.Do(
                    action: () => {
                        LdapServer.Current.EnsureBind(true);
                        return LdapServer.Current.Authenticate(username, password);
                    },
                    retryInterval: TimeSpan.FromSeconds(2), 
                    breakOnValidation: ex => ex.GetType() == typeof(LdapException) && ex.Message.StartsWith("Invalid Credentials"),
                    retryCount: 3))
                {
                    return await Login(username, returnUrl);
                }
            } catch (AggregateException ex)
            {
                logger.Error(ex.InnerExceptions.Last(), WhistlerTypes.LoginError);
                if (ex.InnerExceptions.Last().GetType() == typeof(LdapException) && ex.Message.StartsWith("Invalid Credentials"))
                    ViewData["FailedLoginMessage"] = "Username or password is incorrect";
                else
                {
                    ViewData["FailedLoginMessage"] = ex.InnerExceptions.Last().Message;
                }
            }
            return View();
        }

        private async Task<IActionResult> Login(string email, string returnUrl)
        {
            var groups = (Current.Security as ActiveDirectoryProvider).GetRoles(email);
            var tempUser = new User(email, groups.ToArray());
            await signInManager.SignInAsync(tempUser, true);
            if (returnUrl.HasValue())
                return Redirect(returnUrl);
            else
                return RedirectToAction("index", "a");
        }

        [Route("/logoff")]
        public async Task<IActionResult> Logoff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateEmail(string email)
        {
            var user = uow.Repo<UserProfile>().Get(d => d.Username == Current.User.AccountName).FirstOrDefault();
            if (!user.Email.IsValidEmailAddress())
            {
                HttpContext.Session.AddSnackMessage("Email is invalid");
            }
            else
            {
                user.Email = email;
                uow.Save();
            }
            return RedirectToAction("alerts", "a");
        }
    }
}

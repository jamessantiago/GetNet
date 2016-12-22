using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model;
using Microsoft.AspNetCore.Identity;
using getnet.Model;

namespace getnet.Controllers
{
    public class BaseController : Controller
    {
        protected UnitOfWork uow;
        protected UserManager<User> userManager;
        protected SignInManager<User> signInManager;

        public BaseController(UserManager<User> UserManager,
            SignInManager<User> SignInManager) : this(new UnitOfWork())
        {
            userManager = UserManager;
            signInManager = SignInManager;
        }

        public BaseController(UnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }

        public new RedirectToActionResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }

        #region dispose

        public new void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseController()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (uow != null)
                {
                    uow.Dispose();
                    uow = null;
                }
            }
        }

        #endregion dispose
    }
}

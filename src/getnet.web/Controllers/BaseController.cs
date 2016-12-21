using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model;

namespace getnet.Controllers
{
    public class BaseController : Controller
    {
        protected UnitOfWork uow;

        public BaseController() : this(new UnitOfWork())
        {
        }

        public BaseController(UnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }

        public new RedirectToActionResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
    }
}

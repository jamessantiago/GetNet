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
        protected UnitOfWork _uow;
        protected UnitOfWork uow => _uow ?? (_uow = new UnitOfWork());

        public BaseController() : this(new UnitOfWork())
        {
            
        }

        public BaseController(UnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
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
                if (_uow != null)
                {
                    _uow.Dispose();
                    _uow = null;
                }
            }
        }

        #endregion dispose
    }
}

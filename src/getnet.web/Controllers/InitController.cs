using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model;
using getnet.core.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using getnet.Model;

namespace getnet.Controllers
{
    public class InitController : BaseController
    {
        public InitController(UserManager<User> UserManager,
            SignInManager<User> SignInManager) : base(UserManager, SignInManager)
        { }

        [Route("/configure")]
        public IActionResult Index()
        {
            try {
                ViewData["DbExists"] = uow.CheckIfDabaseExists();
            } catch
            {
                ViewData["DbExists"] = false;
            }
            ViewData["DbError"] = Current.DatabaseConnectionError;
            ViewData["DbString"] = CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString");

            return View();
        }

        [HttpPost]
        public IActionResult Configure(string SqlConfigurationString)
        {
            CoreCurrent.Configuration.SetSecure("Data:SqlServerConnectionString", SqlConfigurationString);
            Current.SetDbConfigurationState();
            uow = new UnitOfWork();
            return PartialView("_success");
        }

        [HttpPost]
        public IActionResult CreateDb()
        {
            Exception e;
            uow.TestDatabaseConnection(out e);
            uow.EnsureDatabaseExists();
            Current.SetDbConfigurationState();
            return PartialView("_success");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using getnet.core.Model;
using getnet.core.Model.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace getnet.Controllers
{
    public class InitController : BaseController
    {
        [Route("/configure")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

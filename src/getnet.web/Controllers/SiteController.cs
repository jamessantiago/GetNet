using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using getnet.core.ssh;

namespace getnet.Controllers
{
    public class SiteController : Controller
    {
        [Route("/newsite")]
        public IActionResult New()
        {
            return View();
        }

        public IActionResult Discover(string hubip)
        {

            var neighbors = hubip.Ssh().Execute<CdpNeighbor>();
            //convert and send
            return PartialView("_newsites");
        }
    }
}

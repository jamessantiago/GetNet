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
using System.Net;

namespace getnet.Controllers
{
    [Authorize]
    public class UtilController : BaseController
    {
        [Route("/ping/{id}")]
        public IActionResult Ping(string id)
        {
            IPAddress ip = null;

            if (IPAddress.TryParse(id, out ip) && ip.Ping())
                return Json(new { success = true });
            else
                return Json(new { success = false });
            
        }
    }
}

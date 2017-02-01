using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using getnet.Model;
using getnet.core.ssh;

namespace getnet.Controllers
{
    [Authorize(Roles =Roles.GlobalAdmins)]
    public class DebugController : Controller
    {
        [HttpGet]
        public IActionResult ssh(string ip, string command)
        {
            return Content(ip.Ssh().Execute<RawSshData>(command).First().Data);
        }
    }
}

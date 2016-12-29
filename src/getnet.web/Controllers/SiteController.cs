using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using getnet.core.ssh;
using getnet.core.Model.Entities;

namespace getnet.Controllers
{
    public class SiteController : BaseController
    {
        [Route("/sites")]
        public IActionResult Sites()
        {
            return View();
        }

        [Route("/s/{id}")]
        public IActionResult Details(string id)
        {
            int siteId;
            if (int.TryParse(id, out siteId))
                return View(uow.Repo<Site>().GetByID(siteId));
            else
                return View(uow.Repo<Site>().Get(d => d.Name == id));
        }

        [Route("/newsite")]
        public IActionResult New()
        {
            return View();
        }

        public IActionResult Discover(string hubip)
        {

            var neighbors = hubip.Ssh().Execute<CdpNeighbor>();
            return PartialView("_newsites", neighbors);
        }

        public void MakeSite(string ip)
        {
            var version = ip.Ssh().Execute<DeviceVersion>()[0];
            var site = new Site()
            {
                Name = version.Hostname + "_Site",
                Priority = Priority.P4
            };
            uow.Repo<Site>().Insert(site);
            uow.Save();
            var siteId = uow.Repo<Site>().Get(d => d.Name == site.Name).First().SiteId;
            HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
            {
                actionHandler = "window.location = '/s/" + siteId.ToString() + "';",
                actionText = "open",
                message = "Successfully created new skeleton site for " + version.Hostname + ".  Open site to configure"
            });
        }
    }
}

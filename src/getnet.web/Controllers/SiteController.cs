using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using getnet.core.ssh;
using getnet.core.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using getnet;

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
            Site site = null;
            if (int.TryParse(id, out siteId))
                site = uow.Repo<Site>().Get(filter: d => d.SiteId == siteId, includeProperties: "Location,HotPaths").FirstOrDefault();
            else
                site = uow.Repo<Site>().Get(filter: d => d.Name == id, includeProperties: "Location,HotPaths").FirstOrDefault();
            return View(site);
        }

        [Route("/newsite")]
        public IActionResult New()
        {
            return View();
        }

        public IActionResult Discover(string hubip)
        {
            ViewData["hubip"] = hubip;
            var neighbors = hubip.Ssh().Execute<CdpNeighbor>();
            return PartialView("_newsites", neighbors);
        }

        public void MakeSite(string ip, string hubip)
        {
            uow.Transaction(() =>
            {
                var version = ip.Ssh().Execute<DeviceVersion>()[0];
                var neighbors = hubip.Ssh().Execute<CdpNeighbor>();
                var thisSite = neighbors.Where(d => d.IP.ToString() == ip).First();
                uow.Repo<Location>().Insert(new Location() { Name = "TestLocale" });

                uow.Repo<HotPath>().Insert(new HotPath() { RawManagementIP = hubip.IpToInt(), Name = thisSite.InPort + "_Circuit", Interface = thisSite.InPort, IsOnline = true });
                uow.Save();
                var site = new Site()
                {
                    Name = version.Hostname + "_Site",
                    Priority = Priority.P4,
                    Location = uow.Repo<Location>().Get(d => d.Name == "TestLocale").First(),
                    HotPaths = uow.Repo<HotPath>().Get(d => d.Name == thisSite.InPort + "_Circuit").ToList()
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
            });            
        }
    }
}

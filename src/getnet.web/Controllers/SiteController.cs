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
using getnet.Model;

namespace getnet.Controllers
{
    public class SiteController : BaseController
    {
        [Route("/sites")]
        public IActionResult Index()
        {
            var sites = uow.Repo<Site>().Get(includeProperties: "Location");
            return View(sites);
        }

        public IActionResult Delete(int id)
        {
            uow.Repo<Site>().Delete(uow.Repo<Site>().Get(filter: d => d.SiteId == id, includeProperties: "HotPaths").First());
            uow.Save();
            return RedirectToAction("index", "site");
        }

        [Route("/s/{id}")]
        public IActionResult Details(string id)
        {
            int siteId;
            Site site = null;
            if (int.TryParse(id, out siteId))
                site = uow.Repo<Site>().Get(filter: d => d.SiteId == siteId, includeProperties: "Location,HotPaths,Priority").FirstOrDefault();
            else
                site = uow.Repo<Site>().Get(filter: d => d.Name == id, includeProperties: "Location,HotPaths,Priority").FirstOrDefault();
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
                var neighbors = ip.Ssh().Execute<CdpNeighbor>();
                var hotpaths = neighbors.Where(d => d.IP.ToString() == ip);
                uow.Repo<Location>().Insert(new Location() { Name = "TestLocale" });

                foreach (var hotpath in hotpaths)
                    uow.Repo<HotPath>().Insert(new HotPath() { RawManagementIP = hotpath.IP.ToInt(), Name = hotpath.OutPort + "_Circuit", Interface = hotpath.OutPort, IsOnline = true });
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

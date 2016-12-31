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
                var router = hubip.Ssh().Execute<CdpNeighbor>()?.Where(d => d.IP.ToInt() == IPAddress.Parse(ip).ToInt()).FirstOrDefault();
                NetworkDevice device = new NetworkDevice()
                {
                    Hostname = router.Hostname,
                    Capabilities = string.Join(",", router.Capabilities),
                    Model = router.Model,
                    RawManagementIP = router.IP.ToInt()
                };
                var devchanges = uow.Repo<NetworkDevice>().Insert(device);
                uow.Save();
                var site = new Site()
                {
                    Name = router.Hostname + "_Site",
                    NetworkDevices = new List<NetworkDevice>() { device }
                };
                uow.Save();

                //capture initial data
                //var version = ip.Ssh().Execute<DeviceVersion>()[0];
                //var neighbors = ip.Ssh().Execute<CdpNeighbor>();
                //var tunnels = neighbors.Where(d => d.OutPort.StartsWith("Tu"));
                //var location = uow.Repo<Location>().Get(d => d.Name == "TestLocale").FirstOrDefault();
                //if (location == null) {
                //    uow.Repo<Location>().Insert(new Location() { Name = "TestLocale" });
                //    uow.Save();
                //    location = uow.Repo<Location>().Get(d => d.Name == "TestLocale").FirstOrDefault();
                //}

                //var hotpaths = new List<HotPath>();
                //foreach (var tunnel in tunnels)
                //{
                //    var change = uow.Repo<HotPath>().Insert(new HotPath() { RawManagementIP = tunnel.IP.ToInt(), Name = tunnel.OutPort, Interface = tunnel.OutPort, IsOnline = true });
                //    uow.Save();
                //    hotpaths.Add(uow.Repo<HotPath>().GetByID((int)change.CurrentValues["HotPathId"]));
                //}


                //site.HotPaths = hotpaths;
                //uow.Repo<Site>().Insert(site);
                //uow.Save();
                var siteId = uow.Repo<Site>().Get(d => d.Name == site.Name).First().SiteId;
                Task.Run(() => ConfigureSite(siteId));
                HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                {
                    actionHandler = "window.location = '/s/" + siteId.ToString() + "';",
                    actionText = "open",
                    message = "Successfully created new skeleton site for " + router.Hostname + ".  Open site to configure"
                });
            });            
        }

        public async void ConfigureSite(int siteId)
        {
            await Task.Run(() => System.Threading.Thread.Sleep(10000));
            //find network devics
            //find hotpaths
            //find vlans
            //find subnets
            //find endpoints
            //diagram?
            //
        }
    }
}

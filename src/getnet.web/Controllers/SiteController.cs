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
using getnet.core.Model;
using getnet.Model;
using getnet.core;

namespace getnet.Controllers
{
    public partial class SiteController : BaseController
    {
        private Whistler logger = new Whistler(typeof(SiteController).FullName);

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
            var includes = "Location,HotPaths,NetworkDevices,Subnets,Vlans,PointOfContacts,Vlans.Devices,Vlans.NetworkDevice";
            if (int.TryParse(id, out siteId))
                site = uow.Repo<Site>().Get(filter: d => d.SiteId == siteId, includeProperties: includes).FirstOrDefault();
            else
                site = uow.Repo<Site>().Get(filter: d => d.Name == id, includeProperties: includes).FirstOrDefault();
            return View(site);
        }

        public IActionResult Edit(int id)
        {
            return View(uow.Repo<Site>().Get(d => d.SiteId == id, includeProperties: "Location").FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Edit(Site site, string Location)
        {
            if (ModelState.IsValid)
            {
                site.Location = uow.Repo<Location>().Get(d => d.Name == Location).FirstOrDefault();
                uow.Repo<Site>().Update(site);
                uow.Save();
                HttpContext.Session.AddSnackMessage("Updates saved");
                return RedirectToAction("Details", new { id = site.Name });
            }
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
            var ipnets = hubip.Ssh().Execute<IpInterface>();
            foreach (var ipnet in ipnets.Where(d => d.IPNetwork.Cidr >= 30 && neighbors.Any(n => n.InPort != d.Interface)))
            {
                var ip = ipnet.IPNetwork.Cidr == 31 ? ipnet.IPNetwork.Broadcast.IncrementIPbyOne() : ipnet.IPNetwork.LastUsable;
                if (ip.CanSsh())
                {
                    var ver = ip.Ssh().Execute<DeviceVersion>();
                    
                    neighbors.Add(new CdpNeighbor
                    {
                        Capabilities = new string[] { "Router" },
                        InPort = ipnet.Interface,
                        OutPort = "Unknown",
                        Hostname = ver.First().Hostname,
                        IP = ip,
                        Model = ver.First().Model
                    });
                }
            }
            var networkdevices = uow.Repo<NetworkDevice>().Get(d => d.Capabilities.HasFlag(NetworkCapabilities.Router));
            neighbors = neighbors.Where(d => d.Capabilities.Contains("Router") && !networkdevices.Any(n => n.RawManagementIP == d.IP.ToInt())).ToList();
            return PartialView("_newsites", neighbors);
        }

        public void MakeSite(string ip)
        {
            try
            {
                int siteId = 0;
                uow.Transaction(() =>
                {
                    var router = ip.Ssh().Execute<DeviceVersion>().First();
                    var testRouter = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == ip.IpToInt(), includeProperties: "Site").FirstOrDefault();
                    if (testRouter != null)
                    {
                        HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                        {
                            message = string.Format("Unable to create site, the router with IP {0} already belongs to site {1}", ip, testRouter.Site.Name)
                        });
                        return;
                    }

                    NetworkDevice device = new NetworkDevice()
                    {
                        Hostname = router.Hostname,
                        Model = router.Model,
                        RawManagementIP = ip.IpToInt(),
                        Capabilities = NetworkCapabilities.Router,
                        ChassisSerial = router.Serial
                    };
                    var devchanges = uow.Repo<NetworkDevice>().Insert(device);
                    uow.Save();
                    var site = new Site()
                    {
                        Name = router.Hostname + "_Site",
                        NetworkDevices = new List<NetworkDevice>() { device }
                    };
                    uow.Repo<Site>().Insert(site);
                    uow.Save();

                    siteId = uow.Repo<Site>().Get(d => d.Name == site.Name).First().SiteId;
                    HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                    {
                        actionHandler = "window.location = '/s/" + siteId.ToString() + "';",
                        actionText = "open",
                        message = "Successfully created new skeleton site for " + router.Hostname + ".  A background job has been initiated to auto-configure the site."
                    });
                });
                Task.Run(() => Discovery.RunFullSiteDiscovery(siteId));
            } catch (Exception ex)
            {
                logger.Error("Failed to configure site", ex, WhistlerTypes.NetworkDiscovery);
                HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                {
                    message = string.Format("An error occured during site creation: {0}", ex.Message)
                });
            }
        }

        public void Rediscover(int id)
        {
            Task.Run(() => Discovery.RunFullSiteDiscovery(id));
        }

    }
}

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

namespace getnet.Controllers
{
    public partial class SiteController : BaseController
    {
        private Whistler logger = new Whistler();

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

        public IActionResult Endpoints(int id)
        {
            return PartialView("_endpoints", uow.Repo<Device>().Get(d => d.Site.SiteId == id));
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
            var networkdevices = uow.Repo<NetworkDevice>().Get(d => d.Capabilities.HasFlag(NetworkCapabilities.Router));
            neighbors = neighbors.Where(d => d.Capabilities.Contains("Router") && !networkdevices.Any(n => n.RawManagementIP == d.IP.ToInt())).ToList();
            return PartialView("_newsites", neighbors);
        }

        public void MakeSite(string ip, string hubip)
        {
            try
            {
                int siteId = 0;
                uow.Transaction(() =>
                {
                    var router = hubip.Ssh().Execute<CdpNeighbor>()?.Where(d => d.IP.ToInt() == IPAddress.Parse(ip).ToInt()).FirstOrDefault();
                    var testRouter = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == router.IP.ToInt(), includeProperties: "Site").FirstOrDefault();
                    if (testRouter != null)
                    {
                        HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                        {
                            message = string.Format("Unable to create site, the router with IP {0} already belongs to site {1}", router.IP.ToString(), testRouter.Site.Name)
                        });
                        return;
                    }

                    NetworkDevice device = new NetworkDevice()
                    {
                        Hostname = router.Hostname,
                        Model = router.Model,
                        RawManagementIP = router.IP.ToInt()
                    };
                    if (router.Capabilities.Contains("Router"))
                        device.Capabilities = NetworkCapabilities.Router;
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
                Task.Run(() => ConfigureSite(siteId));
            } catch (Exception ex)
            {
                logger.Error("Failed to configure site", ex, WhistlerTypes.NetworkDiscovery);
                HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                {
                    message = string.Format("An error occured during site creation: {0}", ex.Message)
                });
            }
        }

        public async void ConfigureSite(int siteId)
        {
            logger.Info("Starting site configuration", WhistlerTypes.NetworkDiscovery, siteId);
            Site site = null;
            using (UnitOfWork buow = new UnitOfWork())
            {
                site = buow.Repo<Site>().Get(d => d.SiteId == siteId, includeProperties: "NetworkDevices").FirstOrDefault();
                if (site == null)
                    return;
            }

            logger.Info("Finding network devices", WhistlerTypes.NetworkDiscovery, siteId);
            await FindNetworkDevices(site);

            site = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "NetworkDevices").First();

            logger.Info("Finding hot paths", WhistlerTypes.NetworkDiscovery, siteId);
            await DiscoverHotPaths(site);

            logger.Info("Finding vlans", WhistlerTypes.NetworkDiscovery, siteId);
            await DiscoverVlans(site);

            logger.Info("Finding subnets", WhistlerTypes.NetworkDiscovery, siteId);
            await DiscoverSubnets(site);

            logger.Info("Finding endpoints", WhistlerTypes.NetworkDiscovery, siteId);
            await DiscoverEndpoints(site);

            HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
            {
                actionHandler = "window.location = '/s/" + siteId.ToString() + "';",
                actionText = "open",
                message = "Completed all discovery actions for the " + site.Name + " site."
            });
        }

        
    }
}

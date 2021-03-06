﻿using System;
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
using getnet.core.Helpers;
using getnet.Helpers;
using Newtonsoft.Json;

namespace getnet.Controllers
{
    [RedirectOnDbIssue]
    [Authorize(Roles=Roles.GlobalViewers)]
    public partial class SiteController : BaseController
    {
        private Whistler logger = new Whistler(typeof(SiteController).FullName);

        public ActionResult SiteHandler(string text, jQueryDataTableParamModel param)
        {
            var predicates = PredicateBuilder.True<Site>();

            if (text.HasValue())
                predicates = predicates.And(Site.SearchPredicates(text));

            var devices = uow.Repo<Site>().Get(predicates, includeProperties: "Location");

            if (param.search["value"].HasValue())
                predicates = predicates.And(Site.SearchPredicates(param.search["value"]));

            var queriedDevices = devices.AsQueryable().Where(predicates);

            Func<Site, string> orderingFunction = (d =>
                param.order[0]["column"] == "0" ? d.Name :
                param.order[0]["column"] == "1" ? d.Owner :
                param.order[0]["column"] == "2" ? d.Location?.Name :
                param.order[0]["column"] == "3" ? d.Building :
                param.order[0]["column"] == "4" ? d.Priority.ToString() :
                param.order[0]["column"] == "5" ? d.Status.ToString() : d.Name);

            if (param.order[0]["dir"] == "asc")
                queriedDevices = queriedDevices.OrderBy(orderingFunction).AsQueryable();
            else
                queriedDevices = queriedDevices.OrderByDescending(orderingFunction).AsQueryable();

            var takecount = param.length;
            var displayedResults = queriedDevices.Skip(param.start).Take(takecount).ToArray();

            var results = from r in displayedResults
                          select new[] {
                              r.Name,
                              r.Owner,
                              r.Location?.Name,
                              r.Building,
                              r.Priority.ToString(),
                              r.Status.ToString()
                          };

            return Json(new
            {
                recordsTotal = devices.Count(),
                recordsFiltered = devices.Count(),
                data = results
            });
        }

        [Route("/sites")]
        public IActionResult Index()
        {
            //var sites = uow.Repo<Site>().Get(includeProperties: "Location");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Roles.GlobalAdmins)]
        public IActionResult Delete(int siteid)
        {
            var includes = "Location,HotPaths,NetworkDevices,Subnets,Vlans,PointOfContacts,Devices,NetworkDevices.RemoteNetworkDeviceConnections,NetworkDevices.LocalNetworkDeviceConnections";
            uow.Repo<Site>().Delete(uow.Repo<Site>().Get(filter: d => d.SiteId == siteid, includeProperties: includes).First());
            uow.Save();
            return RedirectToAction("index", "site");
        }

        public IActionResult SitesPartial(string id)
        {
            ViewData["SearchText"] = id;
            return PartialView("_sitessearch");
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

        [Authorize(Roles = Roles.GlobalAdmins)]
        public IActionResult Edit(int id)
        {
            return View(uow.Repo<Site>().Get(d => d.SiteId == id, includeProperties: "Location").FirstOrDefault());
        }

        [HttpPost]
        [Authorize(Roles = Roles.GlobalAdmins)]
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

        [Route("/discovery")]
        [Authorize(Roles = Roles.GlobalAdmins)]
        public IActionResult New()
        {
            return View();
        }

        [Authorize(Roles = Roles.GlobalAdmins)]
        public IActionResult Discover(string hubip)
        {
            ViewData["hubip"] = hubip;
            var neighbors = hubip.Ssh().Execute<CdpNeighbor>();
            var ipnets = hubip.Ssh().Execute<IpInterface>();
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JsonIpAddressConverter());
            settings.Formatting = Formatting.Indented;
            logger.Info("Discovery performed against " + hubip, 
                JsonConvert.SerializeObject(neighbors, settings) + "\n\n\n\n" + JsonConvert.SerializeObject(ipnets, settings),
                WhistlerTypes.NetworkDiscovery);
            

            foreach (var ipnet in ipnets.Where(d => d.IPNetwork.Cidr >= 30 && neighbors.Any(n => n.InPort != d.Interface)))
            {
                var ip = ipnet.IPNetwork.Cidr == 31 ? ipnet.IP.IncrementIPbyOne() : ipnet.IPNetwork.LastUsable;
                if (!ip.CanSsh()) continue;

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
            var networkdevices = uow.Repo<NetworkDevice>().Get(d => d.Capabilities.HasFlag(NetworkCapabilities.Router));
            neighbors = neighbors.Where(d => d.Capabilities.Contains("Router") && !networkdevices.Any(n => n.RawManagementIP == d.IP.ToInt())).ToList();
            return PartialView("_newsites", neighbors);
        }

        [HttpPost]
        [Authorize(Roles = Roles.GlobalAdmins)]
        public IActionResult AddRouter(int siteid, string ip)
        {
            try
            {
                uow.Transaction(() =>
                {
                    var router = ip.Ssh().Execute<DeviceVersion>().First();
                    var testRouter = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == ip.IpToInt(), includeProperties: "Site").FirstOrDefault();
                    if (testRouter != null)
                    {
                        HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                        {
                            message = string.Format("Unable to add router to site, the router with IP {0} already belongs to site {1}", ip, testRouter.Site.Name)
                        });
                        return;
                    }

                    var site = uow.Repo<Site>().Get(d => d.SiteId == siteid, includeProperties: "NetworkDevices").First();
                    NetworkDevice device = new NetworkDevice()
                    {
                        Hostname = router.Hostname,
                        Model = router.Model,
                        RawManagementIP = ip.IpToInt(),
                        Capabilities = NetworkCapabilities.Router,
                        ChassisSerial = router.Serial,
                        Site = site
                    };

                    if (ip.Ssh().Execute<MacAddress>().Any())
                        device.Capabilities = device.Capabilities | NetworkCapabilities.Switch;
                    var devchanges = uow.Repo<NetworkDevice>().Insert(device);
                    site.NetworkDevices.AddOrNew(device);
                    uow.Save();
                    Task.Run(() => Discovery.RunFullSiteDiscovery(siteid));
                    HttpContext.Session.AddSnackMessage("Successfully added router and initiated a full site rediscovery");
                });
            } catch (Exception ex)
            {
                logger.Error("Failed to configure site", ex, WhistlerTypes.NetworkDiscovery);
                HttpContext.Session.AddSnackMessage(new Model.SnackMessage()
                {
                    message = string.Format("An error occured during site configuration: {0}", ex.Message)
                });

            }
            return Content("");
        }

        [Authorize(Roles = Roles.GlobalAdmins)]
        public IActionResult MakeSite(string ip)
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

                    var site = new Site()
                    {
                        Name = router.Hostname + "_Site",
                        Status = SiteStatus.Online
                    };
                    uow.Repo<Site>().Insert(site);
                    uow.Save();

                    NetworkDevice device = new NetworkDevice()
                    {
                        Hostname = router.Hostname,
                        Model = router.Model,
                        RawManagementIP = ip.IpToInt(),
                        Capabilities = NetworkCapabilities.Router,
                        ChassisSerial = router.Serial,
                        Site = site
                    };
                    if (ip.Ssh().Execute<MacAddress>().Any())
                        device.Capabilities = device.Capabilities | NetworkCapabilities.Switch;
                    
                    var devchanges = uow.Repo<NetworkDevice>().Insert(device);
                    site.NetworkDevices.AddOrNew(device);
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
            return Content("");
        }

        [Authorize(Roles = Roles.GlobalAdmins)]
        public void Rediscover(int id)
        {
            Task.Run(() => Discovery.RunFullSiteDiscovery(id));
        }

        [Authorize(Roles = Roles.GlobalAdmins)]
        public void RediscoverEndpoints(int id)
        {
            if (Discovery.CanRun(id))
            {
                logger.Info("Finding endpoints", WhistlerTypes.NetworkDiscovery, id);
                var site = uow.Repo<Site>().Get(d => d.SiteId == id, includeProperties: "NetworkDevices").FirstOrDefault();
                Task.Run(() =>
                {
                    try
                    {
                        Discovery.DiscoverEndpoints(site);
                        logger.Info("Endpoint discovery complete", WhistlerTypes.NetworkDiscovery, id);
                    }
                    catch (Exception ex) {
                        logger.Error(ex, WhistlerTypes.NetworkDiscovery);
                        logger.Info("Endpoint discovery failed to complete", WhistlerTypes.NetworkDiscovery, id);
                    }
                    finally
                    {
                        Discovery.RunComplete(id);
                    }
                });
            }
        }
    }
}

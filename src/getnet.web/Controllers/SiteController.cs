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
    public class SiteController : BaseController
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
                    message = "Successfully created new skeleton site for " + router.Hostname + ".  Open site to configure"
                });
            });
            Task.Run(() => ConfigureSite(siteId));
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

            logger.Info("Finding hot paths", WhistlerTypes.NetworkDiscovery, siteId);
            await DiscoverHotPaths(site);

            //await uow.TransactionAsync(async () =>
            //{
            //    //find network devics
            //    //find hotpaths
            //    //find vlans
            //    //find subnets
            //    //find endpoints
            //    //diagram?
            //    //
            //});
        }

        public async Task FindNetworkDevices(Site site)
        {
            using (UnitOfWork buow = new UnitOfWork())
            {
                await buow.TransactionAsync(() =>
                {
                    foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                    {
                        UpdateDevice(router);
                        recurseDevices(router, new List<NetworkDevice>());
                    }
                });
            }
        }

        private List<NetworkDevice> recurseDevices(NetworkDevice device, List<NetworkDevice> devices)
        {
            using (UnitOfWork buow = new UnitOfWork())
            {
                var neighbors = device.ManagementIP.Ssh().Execute<CdpNeighbor>().Where(d => !d.Capabilities.GetCaps().HasFlag(NetworkCapabilities.Router));
                foreach (var nei in neighbors)
                {
                    var existingDevice = buow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == nei.IP.ToInt()).FirstOrDefault();
                    if (!devices.Any(d => d.RawManagementIP == nei.IP.ToInt()) && existingDevice == null)
                    {
                        var newDevice = new NetworkDevice
                        {
                            Hostname = nei.Hostname,
                            Model = nei.Model,
                            RawManagementIP = nei.IP.ToInt(),
                            Capabilities = nei.Capabilities.GetCaps()
                        };
                        var newDeviceChanges = buow.Repo<NetworkDevice>().Insert(newDevice);
                        buow.Save();
                        newDevice = buow.Repo<NetworkDevice>().GetByID((int)newDeviceChanges.CurrentValues["NetworkDeviceId"]);
                        buow.Repo<Site>().Get(d => d.SiteId == device.Site.SiteId, includeProperties: "NetworkDevices").First().NetworkDevices.Add(newDevice);
                        buow.Save();
                        var ndc = new NetworkDeviceNetworkDeviceConnection()
                        {
                            NetworkDeviceId = device.NetworkDeviceId,
                            NetworkDevice = device,
                            ConnectedNetworkDeviceId = newDevice.NetworkDeviceId,
                            ConnectedNetworkDevice = newDevice,
                            DevicePort = nei.OutPort,
                            ConnectedDevicePort = nei.InPort
                        };
                        buow.Repo<NetworkDeviceNetworkDeviceConnection>().Insert(ndc);
                        buow.Save();
                        UpdateDevice(newDevice);
                        devices = recurseDevices(newDevice, devices);
                    }
                    else if (existingDevice != null)
                    {
                        existingDevice.Hostname = nei.Hostname;
                        existingDevice.Model = nei.Model;
                        existingDevice.Site = device.Site;
                        existingDevice.Capabilities = nei.Capabilities.GetCaps();
                        buow.Repo<NetworkDevice>().Update(existingDevice);
                        buow.Save();
                        UpdateDevice(existingDevice);
                        devices = recurseDevices(existingDevice, devices);
                    }
                }
                return devices;
            }
        }

        private void UpdateDevice(NetworkDevice device)
        {
            using (UnitOfWork buow = new UnitOfWork())
            {
                var version = device.ManagementIP.Ssh().Execute<DeviceVersion>().First();
                device.ChassisSerial = version.Serial;
                buow.Repo<NetworkDevice>().Update(device);
                buow.Save();
            }
        }

        private async Task DiscoverHotPaths(Site site)
        {
            using (UnitOfWork buow = new UnitOfWork())
            {
                await buow.TransactionAsync(() =>
                {
                    foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                    {
                        foreach (var tunnel in router.ManagementIP.Ssh().Execute<CdpNeighbor>()?.Where(d => d.OutPort.StartsWith("Tu")))
                        {
                            var existingPath = buow.Repo<HotPath>().Get(d => d.Site == site && d.Interface == tunnel.OutPort).FirstOrDefault();
                            if (existingPath == null)
                            {
                                var change = buow.Repo<HotPath>().Insert(new HotPath() {
                                    RawMonitorIP = tunnel.IP.ToInt(),
                                    Name = tunnel.OutPort,
                                    Interface = tunnel.OutPort,
                                    Type = "Tunnel Interface",
                                    IsOnline = true });
                                buow.Save();
                                var dbsite = buow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "HotPaths").First();
                                if (dbsite.HotPaths == null)
                                    dbsite.HotPaths = new List<HotPath> {
                                        buow.Repo<HotPath>().GetByID((int)change.CurrentValues["HotPathId"])};
                                else
                                    dbsite.HotPaths.Add(buow.Repo<HotPath>().GetByID((int)change.CurrentValues["HotPathId"]));
                                buow.Save();
                            }
                            else
                            {
                                //update?
                            }
                        }
                    }
                });
            }
        }
    }
}

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
    public partial class SiteController
    {
        private async Task FindNetworkDevices(Site site)
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
                                var change = buow.Repo<HotPath>().Insert(new HotPath()
                                {
                                    RawMonitorIP = tunnel.IP.ToInt(),
                                    Name = tunnel.OutPort,
                                    Interface = tunnel.OutPort,
                                    Type = "Tunnel Interface",
                                    IsOnline = true
                                });
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
        }  //check out them curlies

        private async Task DiscoverVlans(Site site)
        {
            using (UnitOfWork buow = new UnitOfWork())
            {
                await buow.TransactionAsync(() =>
                {

                });
            }
        }
    }
}

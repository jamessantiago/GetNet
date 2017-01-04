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
using System.Text.RegularExpressions;

namespace getnet.Controllers
{
    public partial class SiteController
    {
        private async Task FindNetworkDevices(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    UpdateDevice(router);
                    recurseDevices(router, new List<NetworkDevice>());
                }
            });
        }

        private List<NetworkDevice> recurseDevices(NetworkDevice device, List<NetworkDevice> devices)
        {
            var neighbors = device.ManagementIP.Ssh().Execute<CdpNeighbor>().Where(d => !d.Capabilities.GetCaps().HasFlag(NetworkCapabilities.Router));
            foreach (var nei in neighbors)
            {
                var existingDevice = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == nei.IP.ToInt()).FirstOrDefault();
                if (!devices.Any(d => d.RawManagementIP == nei.IP.ToInt()) && existingDevice == null)
                {
                    var newDevice = new NetworkDevice
                    {
                        Hostname = nei.Hostname,
                        Model = nei.Model,
                        RawManagementIP = nei.IP.ToInt(),
                        Capabilities = nei.Capabilities.GetCaps()
                    };
                    var newDeviceChanges = uow.Repo<NetworkDevice>().Insert(newDevice);
                    uow.Save();
                    newDevice = uow.Repo<NetworkDevice>().GetByID((int)newDeviceChanges.CurrentValues["NetworkDeviceId"]);
                    uow.Repo<Site>().Get(d => d.SiteId == device.Site.SiteId, includeProperties: "NetworkDevices").First().NetworkDevices.Add(newDevice);
                    uow.Save();
                    var ndc = new NetworkDeviceConnection()
                    {
                        NetworkDeviceId = device.NetworkDeviceId,
                        NetworkDevice = device,
                        ConnectedNetworkDeviceId = newDevice.NetworkDeviceId,
                        ConnectedNetworkDevice = newDevice,
                        DevicePort = nei.OutPort,
                        ConnectedDevicePort = nei.InPort
                    };
                    uow.Repo<NetworkDeviceConnection>().Insert(ndc);
                    uow.Save();
                    UpdateDevice(newDevice);
                    devices = recurseDevices(newDevice, devices);
                }
                else if (existingDevice != null)
                {
                    existingDevice.Hostname = nei.Hostname;
                    existingDevice.Model = nei.Model;
                    existingDevice.Site = device.Site;
                    existingDevice.Capabilities = nei.Capabilities.GetCaps();
                    uow.Repo<NetworkDevice>().Update(existingDevice);
                    uow.Save();
                    UpdateDevice(existingDevice);
                    devices = recurseDevices(existingDevice, devices);
                }
            }
            return devices;
        }

        private void UpdateDevice(NetworkDevice device)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var version = device.ManagementIP.Ssh().Execute<DeviceVersion>().First();
                device.ChassisSerial = version.Serial;
                uow.Repo<NetworkDevice>().Update(device);
                uow.Save();
            }
        }

        private async Task DiscoverHotPaths(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    foreach (var tunnel in router.ManagementIP.Ssh().Execute<CdpNeighbor>()?.Where(d => d.OutPort.StartsWith("Tu")))
                    {
                        var existingPath = uow.Repo<HotPath>().Get(d => d.Site == site && d.Interface == tunnel.OutPort).FirstOrDefault();
                        if (existingPath == null)
                        {
                            var change = uow.Repo<HotPath>().Insert(new HotPath()
                            {
                                RawMonitorIP = tunnel.IP.ToInt(),
                                Name = tunnel.OutPort,
                                Interface = tunnel.OutPort,
                                Type = "Tunnel Interface",
                                IsOnline = true
                            });
                            uow.Save();
                            var dbsite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "HotPaths").First();
                            dbsite.HotPaths.AddOrNew(uow.Repo<HotPath>().GetByID((int)change.CurrentValues["HotPathId"]));
                            uow.Save();
                        }
                        else
                        {
                            //update?
                        }
                    }
                }
            });
        }  //check out them curlies

        private async Task DiscoverVlans(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    var vlans = router.ManagementIP.Ssh().Execute<core.ssh.Vlan>();
                    if (!vlans.Any())
                    {
                        var neighbors = router.ManagementIP.Ssh().Execute<CdpNeighbor>();
                        var subnets = router.ManagementIP.Ssh().Execute<IpInterface>();
                        foreach (var neighbor in neighbors.Where(d => d.Capabilities.Contains("Switch")))
                        {
                            foreach (var subnet in subnets.Where(d => d.Interface == neighbor.InPort))
                            {
                                var changes = uow.Repo<core.Model.Entities.Vlan>().Insert(new core.Model.Entities.Vlan
                                {
                                    VlanNumber = 1,
                                    RawVlanIP = subnet.IPNetwork.Network.ToInt(),
                                    RawVlanSM = subnet.IPNetwork.Netmask.ToInt()
                                });
                                uow.Save();
                                var thisRouter = uow.Repo<NetworkDevice>().Get(d => d.NetworkDeviceId == router.NetworkDeviceId, includeProperties: "Vlans").First();
                                var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Vlans").First();
                                var newVlan = uow.Repo<core.Model.Entities.Vlan>().GetByID((int)changes.CurrentValues["VlanId"]);
                                thisRouter.Vlans.AddOrNew(newVlan);
                                thisSite.Vlans.AddOrNew(newVlan);
                                uow.Save();
                            }
                        }
                    }
                    else
                    {
                        foreach (var vlan in vlans)
                        {
                            var changes = uow.Repo<core.Model.Entities.Vlan>().Insert(new core.Model.Entities.Vlan
                            {
                                VlanNumber = vlan.VlanNumber,
                                RawVlanIP = vlan.IPNetwork.Network.ToInt(),
                                RawVlanSM = vlan.IPNetwork.Netmask.ToInt()
                            });
                            uow.Save();
                            var newVlan = uow.Repo<core.Model.Entities.Vlan>().GetByID((int)changes.CurrentValues["VlanId"]);
                            router.Vlans.Add(newVlan);
                            site.Vlans.Add(newVlan);
                            uow.Save();
                        }
                    }
                }
            });
        }

        private async Task DiscoverSubnets(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Vlans,Subnets").First();
                var oldSubnets = thisSite.Subnets;
                thisSite.Subnets.Clear();
                uow.Save();
                foreach (var vlan in thisSite.Vlans)
                {
                    if (!oldSubnets.Any(d => d.RawSubnetIP == vlan.RawVlanIP && d.RawSubnetSM == d.RawSubnetSM))
                    {
                        var newSub = new Subnet
                        {
                            RawSubnetIP = vlan.RawVlanIP,
                            RawSubnetSM = vlan.RawVlanSM,
                            Type = SubnetTypes.Vlan
                        };
                        thisSite.Subnets.AddOrNew(newSub);
                        uow.Save();
                    }
                }

                //interfaces maybe?
            });
        }

        private async Task DiscoverEndpoints(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    var thisRouter = uow.Repo<NetworkDevice>().Get(d => d.NetworkDeviceId == router.NetworkDeviceId, includeProperties: "RemoteNetworkDeviceConnections").First();
                    var arps = router.ManagementIP.Ssh().Execute<Arp>();
                    var macs = RecurseMacs(thisRouter, new List<MacAddress>());
                    var neis = router.ManagementIP.Ssh().Execute<CdpNeighbor>();
                    foreach (var arp in arps)
                    {
                        var mac = macs.Where(d => d.Mac == arp.Mac).FirstOrDefault();
                        if (mac != null && !Regex.IsMatch(mac.Interface, @"^[VTS]") && !neis.Any(d => d.InPort == mac.Interface || d.OutPort == mac.Interface))
                        {
                            var change = uow.Repo<Device>().Insert(new Device
                            {
                                MAC = macs.Where(d => d.Mac == arp.Mac).FirstOrDefault()?.Mac,
                                RawIP = arp.IP.ToInt(),
                                DiscoveryDate = DateTime.UtcNow,
                                LastSeenOnline = DateTime.UtcNow,
                                Port = macs.Where(d => d.Mac == arp.Mac).FirstOrDefault()?.Interface
                            });
                            uow.Save();
                            var newDevice = uow.Repo<Device>().GetByID((int)change.CurrentValues["DeviceId"]);
                            var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Devices").First();
                            thisSite.Devices.AddOrNew(newDevice);
                            uow.Save();
                        }
                    }
                }
            });
        }

        private List<MacAddress> RecurseMacs(NetworkDevice device, List<MacAddress> macs)
        {
            if (device.Capabilities == NetworkCapabilities.Switch)
            {
                try
                {
                    var devmacs = device.ManagementIP.Ssh().Execute<MacAddress>();
                    foreach (var m in devmacs)
                        macs.Add(new MacAddress
                        {
                            Interface = m.Interface,
                            Mac = m.Mac,
                            TableOwner = device.ManagementIP
                        });
                } catch (Exception ex)
                {
                    logger.Error(ex, WhistlerTypes.NetworkDiscovery);
                }
            }
            var connectedDevices = device.RemoteNetworkDeviceConnections
                ?.Select(d => d.ConnectedNetworkDevice)
                .Where(d => d.Capabilities == NetworkCapabilities.Switch)
                ?? new List<NetworkDevice>().AsEnumerable();

            foreach (var connectedDevice in connectedDevices)
            {
                macs.AddRange(RecurseMacs(connectedDevice, macs));
            }
            return macs;
        }

        private List<CdpNeighbor> RecurseNeighbors(NetworkDevice device, List<CdpNeighbor> neighbors)
        {
            try
            {
                neighbors.AddRange(device.ManagementIP.Ssh().Execute<CdpNeighbor>());
            } catch (Exception ex)
            {
                logger.Error(ex, WhistlerTypes.NetworkDiscovery);
            }
            var connectedDevices = device.RemoteNetworkDeviceConnections.Select(d => d.NetworkDevice).Where(d => d.Capabilities == NetworkCapabilities.Switch);
            foreach (var connectedDevice in connectedDevices)
            {
                neighbors.AddRange(RecurseNeighbors(connectedDevice, neighbors));
            }
            return neighbors;
        }
    }
}

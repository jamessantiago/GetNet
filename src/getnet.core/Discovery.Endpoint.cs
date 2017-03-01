using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace getnet.core
{
    public static partial class Discovery
    {
        public static async Task DiscoverEndpoints(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    var thisRouter = uow.Repo<NetworkDevice>().Get(d => d.NetworkDeviceId == router.NetworkDeviceId, includeProperties: "RemoteNetworkDeviceConnections,RemoteNetworkDeviceConnections.ConnectedNetworkDevice").First();
                    var arps = new List<Arp>();
                    try
                    {
                        arps = router.ManagementIP.Ssh().Execute<Arp>();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Failed to get arp table from router: " + router.Hostname, ex, WhistlerTypes.NetworkDiscovery);
                        continue;
                    }

                    arps = arps.GroupBy(d => d.Mac).Select(d => new Arp
                    {
                        Interface = d.First().Interface,
                        Mac = d.Key,
                        IP = d.First().IP
                    }).ToList();
                    var macs = RecurseMacs(thisRouter, new List<MacAddress>(), 1);
                    var neis = router.ManagementIP.Ssh().Execute<CdpNeighbor>();

                    var sb = new StringBuilder();
                    sb.AppendLine("Arps (Int, Mac, IP)");
                    foreach (var a in arps)
                        sb.AppendFormat("{0}  {1}  {2}\n", a.Interface, a.Mac, a.IP);
                    sb.AppendLine("\nMacs (Int, Mac, Level)");
                    foreach (var a in macs)
                        sb.AppendFormat("{0}  {1}  {2}\n", a.Interface, a.Mac, a.Level);
                    sb.AppendLine("\nNeighbors (IP, InPort, OutPort)");
                    foreach (var a in neis)
                        sb.AppendFormat("{0}  {1}  {2}\n", a.IP, a.InPort, a.OutPort);
                    logger.Info("Collected recursed endpoint data from " + thisRouter.Hostname, sb.ToString(), WhistlerTypes.NetworkDiscovery);


                    foreach (var arp in arps)
                    {
                        var mac = macs.Where(d => d.Mac == arp.Mac)
                            .OrderByDescending(d => d.Level)
                            .ThenBy(d => d.Interface[0])
                            .ThenBy(d => neis.All(f => f.InPort != d.Interface && f.OutPort != d.Interface))
                            .FirstOrDefault();

                        if (mac == null)
                            continue;
                        
                        var thismac = Regex.Replace(mac.Mac, @"\.|:|\-", "").ToUpper();
                        var movedDevice = uow.Repo<Device>().Get(d => d.MAC == thismac && 
                            d.RawIP != arp.IP.ToInt() &&
                            d.Port != macs.FirstOrDefault(m => m.Mac == arp.Mac).Interface, 
                            includeProperties: "DeviceHistories,DeviceHistories.Device").FirstOrDefault();
                        var existingDevice = uow.Repo<Device>().Get(d => d.MAC == thismac).FirstOrDefault();
                        var duplicate = uow.Repo<Device>().Get(d => d.RawIP == arp.IP.ToInt() && d.MAC != thismac).FirstOrDefault();

                        bool createDuplicateIpHistory = duplicate != null;
                        bool createMovedDeviceHistory = movedDevice != null;
                        bool createNewDevice = movedDevice != null || existingDevice == null;
                        bool setDeviceHistories = createDuplicateIpHistory || createMovedDeviceHistory;

                        if (createDuplicateIpHistory)
                        {
                            uow.Repo<Device>().Delete(duplicate);
                            uow.Save();

                            if (duplicate.Type != DeviceType.Reservation)
                            {
                                var newHistory = CreateHistoryFromDevice(duplicate);
                                var histChnge = uow.Repo<DeviceHistory>().Insert(newHistory);
                                uow.Save();
                            }
                        }
                            
                        if (createMovedDeviceHistory)
                        {
                            foreach (var oldhist in movedDevice.DeviceHistories.ToList())
                            {
                                oldhist.Device = null;
                            }
                            uow.Save();
                            uow.Repo<Device>().Delete(movedDevice);
                            uow.Save();

                            var newHistory = CreateHistoryFromDevice(movedDevice);
                            var histChnge = uow.Repo<DeviceHistory>().Insert(newHistory);
                            uow.Save();
                        }

                        if (createNewDevice)
                        {
                            var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Devices,Vlans,Vlans.Devices").First();
                            var change = uow.Repo<Device>().Insert(new Device
                            {
                                MAC = thismac,
                                RawIP = arp.IP.ToInt(),
                                DiscoveryDate = DateTime.UtcNow,
                                LastSeenOnline = DateTime.UtcNow,
                                Port = macs.FirstOrDefault(d => d.Mac == arp.Mac)?.Interface,
                                Site = thisSite
                            });
                            uow.Save();
                            existingDevice = uow.Repo<Device>().GetByID((int)change.CurrentValues["DeviceId"]);

                            thisSite.Vlans.FirstOrDefault(d => IPNetwork.Contains(d.IPNetwork, existingDevice.IP))?.Devices.AddOrNew(existingDevice);
                            thisSite.Devices.AddOrNew(existingDevice);
                            if (existingDevice.Port.HasValue())
                            {
                                var thisNet = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == macs.FirstOrDefault(m => m.Mac == arp.Mac).TableOwner.ToInt()).FirstOrDefault();
                                if (thisNet != null)
                                    existingDevice.NetworkDevice = thisNet;
                            }
                        } else
                        {
                            existingDevice.LastSeenOnline = DateTime.UtcNow;
                            uow.Save();
                        }

                        if (setDeviceHistories)
                        {
                            foreach(var dh in uow.Repo<DeviceHistory>().Get(d => d.MAC == existingDevice.MAC, includeProperties: "Device"))
                            {
                                dh.Device = existingDevice;
                            }

                            uow.Save();
                        }
                        uow.Save();
                        EndpointAnalysis.FullAnalysis(existingDevice.DeviceId);
                        
                    }
                }
            });
        }

        private static DeviceHistory CreateHistoryFromDevice(Device duplicate)
        {
            var newHistory = new DeviceHistory
            {
                DiscoveryDate = duplicate.DiscoveryDate,
                LastSeenOnline = duplicate.LastSeenOnline,
                MAC = duplicate.MAC,
                RawIP = duplicate.RawIP,
                Type = duplicate.Type
            };
            if (duplicate.Details.HasValue())
                newHistory.Details = duplicate.Details;

            if (duplicate.Hostname.HasValue())
                newHistory.Hostname = duplicate.Hostname;

            if (duplicate.PhoneNumber.HasValue())
                newHistory.PhoneNumber = duplicate.PhoneNumber;

            if (duplicate.Port.HasValue())
                newHistory.Port = duplicate.Port;

            if (duplicate.SerialNumber.HasValue())
                newHistory.SerialNumber = duplicate.SerialNumber;

            if (duplicate.Tenant != null)
                newHistory.Tenant = duplicate.Tenant;

            return newHistory;
        }

        private static List<MacAddress> RecurseMacs(NetworkDevice device, List<MacAddress> macs, int level)
        {
            if (device.Capabilities.HasFlag(NetworkCapabilities.Switch))
            {
                try
                {
                    var devmacs = device.ManagementIP.Ssh().Execute<MacAddress>();
                    foreach (var m in devmacs)
                        macs.Add(new MacAddress
                        {
                            Interface = m.Interface,
                            Mac = m.Mac,
                            TableOwner = device.ManagementIP,
                            Level = level
                        });
                }
                catch (Exception ex)
                {
                    logger.Error(ex, WhistlerTypes.NetworkDiscovery);
                }
            }
            var connectedDevices = device.RemoteNetworkDeviceConnections
                ?.Select(d => d.ConnectedNetworkDevice)
                .Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Switch))
                ?? new List<NetworkDevice>().AsEnumerable();

            foreach (var connectedDevice in connectedDevices)
            {
                macs.AddRange(RecurseMacs(connectedDevice, macs, level++));
            }
            return macs;
        }

        private static List<CdpNeighbor> RecurseNeighbors(NetworkDevice device, List<CdpNeighbor> neighbors)
        {
            try
            {
                neighbors.AddRange(device.ManagementIP.Ssh().Execute<CdpNeighbor>());
            }
            catch (Exception ex)
            {
                logger.Error(ex, WhistlerTypes.NetworkDiscovery);
            }
            var connectedDevices = device.RemoteNetworkDeviceConnections.Select(d => d.NetworkDevice).Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Switch));
            foreach (var connectedDevice in connectedDevices)
            {
                neighbors.AddRange(RecurseNeighbors(connectedDevice, neighbors));
            }
            return neighbors;
        }
    }
}

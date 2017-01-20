using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;
using System.Text.RegularExpressions;
using System.Net;

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
                    var thisRouter = uow.Repo<NetworkDevice>().Get(d => d.NetworkDeviceId == router.NetworkDeviceId, includeProperties: "RemoteNetworkDeviceConnections").First();
                    var arps = router.ManagementIP.Ssh().Execute<Arp>();
                    var macs = RecurseMacs(thisRouter, new List<MacAddress>());
                    var neis = router.ManagementIP.Ssh().Execute<CdpNeighbor>();
                    foreach (var arp in arps)
                    {
                        var mac = macs.Where(d => d.Mac == arp.Mac).FirstOrDefault();
                        if (mac != null && !Regex.IsMatch(mac.Interface, @"^[VTS]") && !neis.Any(d => d.InPort == mac.Interface || d.OutPort == mac.Interface))
                        {
                            var thismac = Regex.Replace(macs.Where(d => d.Mac == arp.Mac).FirstOrDefault()?.Mac, @"\.|:|\-", "");
                            var existingDevice = uow.Repo<Device>().Get(d => d.MAC == thismac && d.RawIP != arp.IP.ToInt()).FirstOrDefault();

                            if (existingDevice != null)
                            {
                                uow.Repo<Device>().Delete(existingDevice);
                                uow.Save();
                            }

                            var change = uow.Repo<Device>().Insert(new Device
                            {
                                MAC = thismac,
                                RawIP = arp.IP.ToInt(),
                                DiscoveryDate = DateTime.UtcNow,
                                LastSeenOnline = DateTime.UtcNow,
                                Port = macs.Where(d => d.Mac == arp.Mac).FirstOrDefault()?.Interface
                            });
                            uow.Save();
                            var newDevice = uow.Repo<Device>().GetByID((int)change.CurrentValues["DeviceId"]);
                            var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Devices,Vlans,Vlans.Devices").First();
                            thisSite.Vlans.FirstOrDefault(d => IPNetwork.Contains(d.IPNetwork, newDevice.IP))?.Devices.AddOrNew(newDevice);
                            thisSite.Devices.AddOrNew(newDevice);
                            if (newDevice.Port.HasValue())
                            {
                                var thisNet = uow.Repo<NetworkDevice>().Get(d => d.RawManagementIP == macs.Where(m => m.Mac == arp.Mac).FirstOrDefault().TableOwner.ToInt()).FirstOrDefault();
                                if (thisNet != null)
                                    newDevice.NetworkDevice = thisNet;
                            }
                            if (existingDevice != null)
                            {
                                newDevice.DeviceHistories.Add(new DeviceHistory
                                {
                                    Details = existingDevice.Details,
                                    Hostname = existingDevice.Hostname,
                                    DiscoveryDate = existingDevice.DiscoveryDate,
                                    LastSeenOnline = existingDevice.LastSeenOnline,
                                    MAC = existingDevice.MAC,
                                    PhoneNumber = existingDevice.PhoneNumber,
                                    Port = existingDevice.Port,
                                    RawIP = existingDevice.RawIP,
                                    SerialNumber = existingDevice.SerialNumber,
                                    Tenant = existingDevice.Tenant,
                                    Type = existingDevice.Type
                                });
                                uow.Save();
                            }
                            uow.Save();
                            
                                
                        }
                    }
                }
            });
        }

        private static List<MacAddress> RecurseMacs(NetworkDevice device, List<MacAddress> macs)
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
                }
                catch (Exception ex)
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
            var connectedDevices = device.RemoteNetworkDeviceConnections.Select(d => d.NetworkDevice).Where(d => d.Capabilities == NetworkCapabilities.Switch);
            foreach (var connectedDevice in connectedDevices)
            {
                neighbors.AddRange(RecurseNeighbors(connectedDevice, neighbors));
            }
            return neighbors;
        }
    }
}

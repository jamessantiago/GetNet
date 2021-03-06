﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;
using System.Net;

namespace getnet.core
{
    public static partial class Discovery
    {
        public static async Task DiscoverVlans(Site site)
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
                                if (uow.Repo<Model.Entities.Vlan>().Get(d => d.RawVlanIP == subnet.IPNetwork.Network.ToInt() && d.RawVlanSM == subnet.IPNetwork.Netmask.ToInt()).Any())
                                    continue;
                                var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Vlans").FirstOrDefault();
                                var thisRouter = uow.Repo<NetworkDevice>().Get(d => d.NetworkDeviceId == router.NetworkDeviceId, includeProperties: "Vlans").FirstOrDefault();
                                uow.Repo<Model.Entities.Vlan>().Insert(new Model.Entities.Vlan
                                {
                                    VlanNumber = 1,
                                    RawVlanIP = subnet.IPNetwork.Network.ToInt(),
                                    RawVlanSM = subnet.IPNetwork.Netmask.ToInt(),
                                    NetworkDevice = thisRouter,
                                    Site = thisSite
                                });
                                uow.Save();
                            }
                        }
                    }
                    else
                    {
                        foreach (var vlan in vlans)
                        {
                            if (uow.Repo<Model.Entities.Vlan>().Get(d => d.RawVlanIP == vlan.IPNetwork.Network.ToInt() && d.RawVlanSM == vlan.IPNetwork.Netmask.ToInt()).Any())
                                continue;
                            var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Vlans").FirstOrDefault();
                            var thisRouter = uow.Repo<NetworkDevice>().Get(d => d.NetworkDeviceId == router.NetworkDeviceId, includeProperties: "Vlans").FirstOrDefault();
                            uow.Repo<Model.Entities.Vlan>().Insert(new Model.Entities.Vlan
                            {
                                VlanNumber = vlan.VlanNumber,
                                RawVlanIP = vlan.IPNetwork.Network.ToInt(),
                                RawVlanSM = vlan.IPNetwork.Netmask.ToInt(),
                                NetworkDevice = thisRouter,
                                Site = thisSite
                            });
                            uow.Save();
                        }
                    }


                    var currentVlans = uow.Repo<Model.Entities.Vlan>().Get(d => d.NetworkDevice == router).ToList();
                    var vlansToRemove = currentVlans.Where(d => d.VlanNumber != 1 && !vlans.Any(v => v.IPNetwork.Network.ToInt() == d.RawVlanIP && v.IPNetwork.Netmask.ToInt() == d.RawVlanSM));
                    var vlansToKeep = currentVlans.Where(d => !vlansToRemove.Contains(d));

                    foreach (var oldvlan in vlansToRemove)
                    {
                        var devicesToMove = uow.Repo<Device>().Get(d => d.Vlan == oldvlan || d.Vlan == null, includeProperties: "Vlan").ToList();
                        foreach (var dev in devicesToMove)
                            dev.Vlan = vlansToKeep.FirstOrDefault(d => IPNetwork.Contains(d.IPNetwork, dev.IP));
                        router.Vlans.Remove(oldvlan);
                        uow.Save();
                    }
                }
            });
        }
    }
}

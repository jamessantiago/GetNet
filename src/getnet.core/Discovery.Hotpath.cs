﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;

namespace getnet.core
{
    public static partial class Discovery
    {
        public static async Task DiscoverHotPaths(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    var tunnels = router.ManagementIP.Ssh().Execute<CdpNeighbor>()?.Where(d => d.OutPort.StartsWith("Tu"));
                    var tunnelmonitors = new Dictionary<long, CdpNeighbor>();
                    foreach (var tunnel in tunnels)
                    {
                        long thisIp = 0;
                        try
                        {
                            var ipints = tunnel.IP.Ssh().Execute<IpInterface>();
                            var tunnelip = ipints.FirstOrDefault(d => d.Interface.StartsWith("l", StringComparison.CurrentCultureIgnoreCase));
                            thisIp = tunnelip != null ? tunnelip.IP.ToInt() : tunnel.IP.ToInt();
                            tunnelmonitors.Add(thisIp, tunnel);
                        }
                        catch { }
                        var existingPath = uow.Repo<HotPath>().Get(d => d.Site == site && d.Interface == tunnel.OutPort && d.RawMonitorIP == thisIp).FirstOrDefault();
                        if (existingPath == null)
                        {
                            var dbsite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "HotPaths").First();
                            var change = uow.Repo<HotPath>().Insert(new HotPath()
                            {
                                MonitorDeviceHostname = tunnel.Hostname,
                                RawMonitorIP = thisIp,
                                Name = tunnel.OutPort,
                                Interface = tunnel.OutPort,
                                Type = HotpathType.Tunnel,
                                Status = HotPathStatus.Online,
                                Site = dbsite
                            });
                            uow.Save();
                            dbsite.HotPaths.AddOrNew(uow.Repo<HotPath>().GetByID((int)change.CurrentValues["HotPathId"]));
                            uow.Save();
                        }
                        else
                        {
                            existingPath.MonitorDeviceHostname = tunnel.Hostname;
                            existingPath.Status = HotPathStatus.Online;
                            existingPath.RawMonitorIP = thisIp;
                            uow.Save();
                        }
                    }
                    var paths = uow.Repo<Site>().Get(d => d == site, includeProperties: "HotPaths").FirstOrDefault().HotPaths?.ToList();
                    var oldpaths = paths?.Where(d => !tunnelmonitors.Any(t => t.Key == d.RawMonitorIP && t.Value.OutPort == d.Interface));
                    foreach (var oldpath in oldpaths)
                    {
                        uow.Repo<HotPath>().Delete(oldpath);
                        uow.Save();
                    }
                }
            });
        }
    }
}
using System;
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
                    foreach (var tunnel in router.ManagementIP.Ssh().Execute<CdpNeighbor>()?.Where(d => d.OutPort.StartsWith("Tu")))
                    {
                        long thisIp = 0;
                        try
                        {
                            var ipints = tunnel.IP.Ssh().Execute<IpInterface>();
                            var tunnelip = ipints.FirstOrDefault(d => d.Interface.StartsWith("l", StringComparison.CurrentCultureIgnoreCase));
                            thisIp = tunnelip != null ? tunnelip.IP.ToInt() : tunnel.IP.ToInt();
                        }
                        catch { }
                        var existingPath = uow.Repo<HotPath>().Get(d => d.Site == site && d.Interface == tunnel.OutPort && d.RawMonitorIP == thisIp).FirstOrDefault();
                        if (existingPath == null)
                        {
                            var change = uow.Repo<HotPath>().Insert(new HotPath()
                            {
                                MonitorDeviceHostname = tunnel.Hostname,
                                RawMonitorIP = thisIp,
                                Name = tunnel.OutPort,
                                Interface = tunnel.OutPort,
                                Type = HotpathType.Tunnel,
                                Status = HotPathStatus.Online
                            });
                            uow.Save();
                            var dbsite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "HotPaths").First();
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
                }
            });
        }  //check out them curlies
    }
}

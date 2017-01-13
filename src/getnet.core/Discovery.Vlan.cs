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
    }
}

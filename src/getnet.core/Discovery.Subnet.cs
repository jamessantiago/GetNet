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
        public static async Task DiscoverSubnets(Site site)
        {
            await uow.TransactionAsync(() =>
            {
                var thisSite = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "Vlans,Subnets").First();
                var oldSubnets = thisSite.Subnets;
                foreach (var sub in oldSubnets)
                {
                    uow.Repo<Subnet>().Delete(sub);
                }
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
                foreach (var router in site.NetworkDevices.Where(d => d.Capabilities.HasFlag(NetworkCapabilities.Router)))
                {
                    try
                    {
                        var ints = router.ManagementIP.Ssh().Execute<IpInterface>();
                        var intsToAdd = new List<IpInterface>();
                        foreach (var i in ints)
                        {
                            if (!oldSubnets.Any(d => d.RawSubnetIP == i.IPNetwork.Network.ToInt() && d.RawSubnetSM == d.IPNetwork.Netmask.ToInt()))
                            {
                                var newSub = new Subnet
                                {
                                    RawSubnetIP = i.IPNetwork.Network.ToInt(),
                                    RawSubnetSM = i.IPNetwork.Netmask.ToInt()
                                };
                                switch (i.Interface[0].ToString().ToLower())
                                {
                                    case "l":
                                        newSub.Type = SubnetTypes.Loopback;
                                        thisSite.Subnets.AddOrNew(newSub);
                                        break;
                                    default:
                                        newSub.Type = SubnetTypes.Interface;
                                        thisSite.Subnets.AddOrNew(newSub);
                                        break;
                                }
                            }
                        }
                        uow.Save();
                    }
                    catch
                    {

                    }
                }

                //interfaces maybe?
            });
        }
    }
}

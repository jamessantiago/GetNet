using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;
using System.Net;
using getnet.core;
using System.Threading;

namespace getnet.service.Jobs
{
    public class FullSiteDiscoveryJob : IJob
    {
        private Whistler logger = new Whistler(typeof(HotpathJob).FullName);

        public virtual Task Execute(IJobExecutionContext context)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                foreach (var site in uow.Repo<Site>().Get(d => d.NetworkDevices.Any(n => n.Capabilities.HasFlag(NetworkCapabilities.Router))))
                {
                    logger.Info("Full site discovery called by getnet.service for " + site.Name, WhistlerTypes.NetworkDiscovery, site.SiteId);
                    Task.Run(() => SiteDiscovery(site, uow));
                    Thread.Sleep(10000);
                }
            }
            return Task.FromResult(0);
        }

        private async Task SiteDiscovery(Site site, UnitOfWork uow)
        {
            int siteId = site.SiteId;
            logger.Info("Finding network devices", WhistlerTypes.NetworkDiscovery, siteId);
            await Discovery.FindNetworkDevices(site);

            site = uow.Repo<Site>().Get(d => d.SiteId == site.SiteId, includeProperties: "NetworkDevices").First();

            logger.Info("Finding hot paths", WhistlerTypes.NetworkDiscovery, siteId);
            await Discovery.DiscoverHotPaths(site);

            logger.Info("Finding vlans", WhistlerTypes.NetworkDiscovery, siteId);
            await Discovery.DiscoverVlans(site);

            logger.Info("Finding subnets", WhistlerTypes.NetworkDiscovery, siteId);
            await Discovery.DiscoverSubnets(site);

            logger.Info("Finding endpoints", WhistlerTypes.NetworkDiscovery, siteId);
            await Discovery.DiscoverEndpoints(site);

            logger.Info("Completed network discovery actions", WhistlerTypes.NetworkDiscovery, siteId);
        }
    }
}

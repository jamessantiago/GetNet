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
        private static UnitOfWork uow = new UnitOfWork();
        private static Whistler logger = new Whistler(typeof(Discovery).FullName);

        public async static void RunFullSiteDiscovery(int siteId)
        {
            logger.Info("Starting site configuration", WhistlerTypes.NetworkDiscovery, siteId);
            Site site = null;
            site = uow.Repo<Site>().Get(d => d.SiteId == siteId, includeProperties: "NetworkDevices").FirstOrDefault();
            if (site == null)
                return;

            try
            {
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

                //todo DHCP

                //todo sites and services

                logger.Info("Completed all site discovery actions.  Refresh the site details page to view added items", WhistlerTypes.NetworkDiscovery, siteId);

            }
            catch (Exception ex)
            {
                logger.Error("Failed to complete all network discovery actions", ex, WhistlerTypes.NetworkDiscovery);
            }
        }
    }
}

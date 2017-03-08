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
    public class EndpointRediscoveryJob : IJob
    {
        private Whistler logger = new Whistler(typeof(HotpathJob).FullName);

        public virtual Task Execute(IJobExecutionContext context)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                foreach (var site in uow.Repo<Site>().Get(d => d.Status > SiteStatus.Offline && d.NetworkDevices.Any(n => n.Capabilities.HasFlag(NetworkCapabilities.Router)),
                    includeProperties: "NetworkDevices"))
                {
                    if (Discovery.CanRun(site.SiteId))
                    {
                        logger.Info("Finding endpoints", WhistlerTypes.NetworkDiscovery, site.SiteId);
                        Task.Run(() =>
                        {
                            try
                            {
                                Discovery.DiscoverEndpoints(site);
                                logger.Info("Endpoint discovery complete", WhistlerTypes.NetworkDiscovery, site.SiteId);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, WhistlerTypes.NetworkDiscovery);
                                logger.Info("Endpoint discovery failed to complete", WhistlerTypes.NetworkDiscovery, site.SiteId);
                            }
                            finally
                            {
                                Discovery.RunComplete(site.SiteId);
                            }
                        });
                    }
                }
            }

            return Task.FromResult(0);
        }
    }
}

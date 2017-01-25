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
            List<Task> tasks = new List<Task>();
            using (UnitOfWork uow = new UnitOfWork())
            {
                foreach (var site in uow.Repo<Site>().Get(d => d.Status > SiteStatus.Offline && d.NetworkDevices.Any(n => n.Capabilities.HasFlag(NetworkCapabilities.Router)), 
                    includeProperties: "NetworkDevices"))
                {
                    logger.Info("Full site discovery called by getnet.service for " + site.Name, WhistlerTypes.NetworkDiscovery, site.SiteId);
                    tasks.Add(Task.Run(() => Discovery.RunFullSiteDiscovery(site.SiteId)));
                }
                
                foreach (var task in tasks)
                {
                    try
                    {
                        task.Wait(600000);
                    } catch (Exception ex)
                    {
                        logger.Error(ex, WhistlerTypes.ServiceControl);
                    }
                }
                    
            }
            return Task.FromResult(0);
        }
    }
}

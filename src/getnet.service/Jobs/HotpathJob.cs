using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;
using System.Net;

namespace getnet.service.Jobs
{
    public class HotpathJob : IJob
    {
        private Whistler logger = new Whistler(typeof(HotpathJob).FullName);

        public virtual Task Execute(IJobExecutionContext context)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                logger.Info("Running hotpath checks", WhistlerTypes.ServiceScheduling);
                var hotpaths = uow.Repo<HotPath>().Get(d => d.Type == HotpathType.Tunnel && d.Site.Status != SiteStatus.Maintenance, includeProperties: "Site").ToList();
                var tunnelIps = hotpaths.Select(d => d.RawMonitorIP).Distinct();
                Dictionary<long, List<InterfaceDescription>> ints = new Dictionary<long, List<InterfaceDescription>>();
                foreach (var hub in tunnelIps)
                {
                    try
                    {
                        var data = (new IPAddress(hub)).Ssh().Execute<InterfaceDescription>();
                        ints.Add(hub, data);
                    } catch (Exception ex)
                    {
                        logger.Error(ex, WhistlerTypes.ServiceScheduling);
                    }
                }
                foreach (var hotpath in hotpaths)
                {
                    try
                    {
                        var hotpathint = ints.FirstOrDefault(d => d.Key == hotpath.RawMonitorIP).Value.FirstOrDefault(d => d.Interface.IntMatch(hotpath.Interface));
                        hotpath.Status = hotpathint.IsUp ? HotPathStatus.Online : HotPathStatus.Offline;
                        uow.Save();
                    } catch (Exception ex)
                    {
                        try
                        {
                            hotpath.Status = HotPathStatus.Unknown;
                            uow.Save();
                        }
                        catch { }
                        logger.Error(ex, WhistlerTypes.ServiceScheduling);
                    }
                }
                foreach (var site in hotpaths.Select(d => d.Site).Distinct())
                {
                    try
                    {
                        if (site.HotPaths.AllOnline())
                            site.Status = SiteStatus.Online;
                        else if (site.HotPaths.IsDegraded())
                            site.Status = SiteStatus.Degraded;
                        else if (site.HotPaths.AllOffline())
                            site.Status = SiteStatus.Offline;
                        uow.Save();
                    } catch (Exception ex)
                    {
                        logger.Error(ex, WhistlerTypes.ServiceScheduling);
                    }
                }
            }
            return Task.FromResult(0);
        }
    }
}

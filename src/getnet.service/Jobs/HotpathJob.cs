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
        private Whistler logger = new Whistler();

        public virtual Task Execute(IJobExecutionContext context)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var hotpaths = uow.Repo<HotPath>().Get(d => d.Type == HotpathType.Tunnel);
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

                    }
                }
                foreach (var hotpath in hotpaths)
                {
                    try
                    {
                        var hotpathint = ints.FirstOrDefault(d => d.Key == hotpath.RawMonitorIP).Value.FirstOrDefault(d => d.Interface == hotpath.Interface);
                        hotpath.IsOnline = hotpathint.IsUp;
                        uow.Repo<HotPath>().Update(hotpath);
                        uow.Save();
                    } catch (Exception ex)
                    {

                    }
                }
            }
            return Task.FromResult(0);
        }
    }
}

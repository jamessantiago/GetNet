using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using getnet.core.Model;
using getnet.core.Model.Entities;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using getnet.service.Jobs;

namespace getnet.service
{
    public static class Runner
    {
        private static AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private static Whistler logger = new Whistler(typeof(Runner).FullName);

        public static async void Run()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            Current.Scheduler = await sf.GetScheduler();
            using (UnitOfWork uow = new UnitOfWork())
            {
                var tasks = uow.Repo<TaskSchedule>().Get();
                if (!tasks.Any())
                {
                    AddDefaultTasks(uow);
                    tasks = uow.Repo<TaskSchedule>().Get();
                }

                foreach (var task in tasks)
                {
                    if (!task.Enabled)
                        continue;

                    IJobDetail job = null;

                    switch (task.Type)
                    {
                        case ScheduleType.HotpathCheck:
                            job = JobBuilder.Create<HotpathJob>()
                                .WithIdentity(task.Name)
                                .Build();
                            break;
                        case ScheduleType.FullSiteDiscovery:
                            job = JobBuilder.Create<FullSiteDiscoveryJob>()
                                .WithIdentity(task.Name)
                                .Build();
                            break;
                        default:
                            break;
                    }

                    ITrigger trigger = TriggerBuilder.Create()
                                .WithIdentity(task.Name)
                                //.StartNow()
                                .WithCronSchedule(task.CronSchedule)
                                .Build();

                    logger.Info("Setting schedule for " + task.Name + " at " + task.CronSchedule, WhistlerTypes.ServiceScheduling);
                    await Current.Scheduler.ScheduleJob(job, trigger);
                }
            }

            await Current.Scheduler.Start();

            try { _resetEvent.WaitOne(); } catch { }
            await Current.Scheduler.Shutdown();
        }

        private static void AddDefaultTasks(UnitOfWork uow)
        {
            uow.Repo<TaskSchedule>().Insert(new TaskSchedule
            {
                Name = "Hotpath Checks Every 5 Min",
                Enabled = true,
                CronSchedule = "0 0/5 * * * ?",
                Type = ScheduleType.HotpathCheck
            });
            uow.Repo<TaskSchedule>().Insert(new TaskSchedule
            {
                Name = "Site Rediscovery Every Saturday at 12PM",
                Enabled = true,
                CronSchedule = "0 0 12 ? * SAT *",
                Type = ScheduleType.FullSiteDiscovery
            });
            uow.Save();
        }
    }
}

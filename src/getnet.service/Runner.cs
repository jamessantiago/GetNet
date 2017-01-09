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
        private static Whistler logger = new Whistler();

        public static async void Run()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            Current.Scheduler = await sf.GetScheduler();
            UnitOfWork uow = new UnitOfWork();

            foreach (var task in uow.Repo<TaskSchedule>().Get())
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
                    default:
                        break;
                }

                ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity(task.Name)
                            .WithCronSchedule(task.CronSchedule)
                            .Build();

                logger.Info("Setting schedule for " + task.Name + " at " + task.CronSchedule, WhistlerTypes.ServiceScheduling);
                await Current.Scheduler.ScheduleJob(job, trigger);
            }

            await Current.Scheduler.Start();

            _resetEvent.WaitOne();
            await Current.Scheduler.Shutdown();
            uow.Dispose();
        }
    }
}

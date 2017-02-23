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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Sockets;

namespace getnet.service
{
    public static class Runner
    {
        public static AutoResetEvent ResetEvent = new AutoResetEvent(false);
        private static Whistler logger = new Whistler(typeof(Runner).FullName);

        public static void Run()
        {
            Console.CancelKeyPress -= ConsoleOnCancelKeyPress;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5002")
                .Build();

            ISchedulerFactory sf = new StdSchedulerFactory();
            Current.Scheduler = sf.GetScheduler().Result;
            using (UnitOfWork uow = new UnitOfWork())
            {
                Exception dbTest;
                if (!uow.TestDatabaseConnection(out dbTest))
                {
                    logger.Error(dbTest, WhistlerTypes.ServiceControl);
                }

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
                        case ScheduleType.EndpointDiscovery:
                            job = JobBuilder.Create<EndpointRediscoveryJob>()
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
                    Current.Scheduler.ScheduleJob(job, trigger);
                }
            }

            logger.Info("Starting scheduler", WhistlerTypes.ServiceScheduling);
            Task.Run(() => Current.Scheduler.Start());

            logger.Info("Starting nancy API router against http://localhost:5002", WhistlerTypes.ServiceControl);
            Console.WriteLine("Default API key: {0}", CoreCurrent.Configuration.GetSecure("Api:Keys:Default"));
            host.Start();
            
            try {
                ResetEvent.WaitOne();
                logger.Info("Shutting down", WhistlerTypes.ServiceControl);
            } catch (Exception ex)
            {
                logger.Error(ex, WhistlerTypes.ServiceControl);
            }
            Current.Scheduler.Shutdown();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            ResetEvent.Set();
        }

        public static void Restart(int sleep = 5)
        {
            ResetEvent.Set();
            Thread.Sleep(sleep);
            Run();
        }

        public static void Shutdown()
        {
            ResetEvent.Set();
        }

        private static void AddDefaultTasks(UnitOfWork uow)
        {
            uow.Repo<TaskSchedule>().Insert(new TaskSchedule
            {
                Name = "Hotpath Checks Every 7 Min",
                Enabled = true,
                CronSchedule = "0 0/7 * * * ?",
                Type = ScheduleType.HotpathCheck
            });
            uow.Repo<TaskSchedule>().Insert(new TaskSchedule
            {
                Name = "Site Rediscovery Every Saturday at 12PM",
                Enabled = true,
                CronSchedule = "0 0 12 ? * SAT *",
                Type = ScheduleType.FullSiteDiscovery
            });
            uow.Repo<TaskSchedule>().Insert(new TaskSchedule
            {
                Name = "Endpoint Rediscovery Every 7 hours",
                Enabled = true,
                CronSchedule = "0 0 0/7 1/1 * ? *",
                Type = ScheduleType.EndpointDiscovery
            });
            uow.Save();
        }
    }
}

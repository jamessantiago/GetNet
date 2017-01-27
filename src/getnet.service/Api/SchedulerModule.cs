using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Quartz.Impl.Matchers;
using System.Threading;
using Quartz;
using Nancy.ModelBinding;

namespace getnet.service.Api
{
    public class SchedulerModule : NancyModule
    {
        public SchedulerModule() : base("/scheduler")
        {
            Get("/jobs", args => GetJobDetails());
            Get("/triggers", args => GetTriggerDetails());
            Post("/run", args =>
            {
                Job jobToRun = this.Bind<Job>();
                RunJob(jobToRun);
                return string.Empty;
            });
        }

        public IEnumerable<IJobDetail> GetJobDetails()
        {
            foreach (var job in Current.Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result)
                yield return Current.Scheduler.GetJobDetail(job).Result;
        }

        public IEnumerable<ITrigger> GetTriggerDetails()
        {
            foreach (var trigger in Current.Scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).Result)
                yield return Current.Scheduler.GetTrigger(trigger).Result;
        }

        public void RunJob(Job jobToRun)
        {
            var job = Current.Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result.Where(d => d.Name == jobToRun.Name).FirstOrDefault();
            if (job != null)
                Current.Scheduler.TriggerJob(job);
        }
    }
}

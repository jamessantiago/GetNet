using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;

namespace getnet.service
{
    public static class Current
    {
        public static IScheduler Scheduler { get; internal set; }
    }
}

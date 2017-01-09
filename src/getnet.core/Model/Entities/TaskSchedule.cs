using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.Model.Entities
{
    public class TaskSchedule
    {
        public int TaskScheduleId { get; set; }
        public ScheduleType Type { get; set; }
        public string Name { get; set; }
        public string CronSchedule { get; set; }
        public bool Enabled { get; set; }
    }
}

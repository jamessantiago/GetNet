using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace getnet.core.Model.Entities
{
    public class HotPath
    {
        public int HotPathId { get; set; }

        [NotMapped]
        public IPAddress MonitorIP => new IPAddress(RawMonitorIP);

        public string MonitorDeviceHostname { get; set; }
        public long RawMonitorIP { get; set; }
        public string Interface { get; set; }

        public HotpathType Type { get; set; }

        public string Name { get; set; }

        public HotPathStatus Status { get; set; }
        
        public Site Site { get; set; }
    }
}

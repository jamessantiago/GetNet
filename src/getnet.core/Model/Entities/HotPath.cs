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
        public IPAddress ManagementIP => new IPAddress(RawManagementIP);

        public int RawManagementIP { get; set; }
        public string Interface { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public bool IsOnline { get; set; }

        public Site Site { get; set; }
    }
}

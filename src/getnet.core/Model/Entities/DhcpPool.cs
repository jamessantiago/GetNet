using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace getnet.core.Model.Entities
{
    public class DhcpPool
    {
        public int DhcpPoolId { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        public int RawStartIP { get; set; }
        public int RawEndIP { get; set; }

        [NotMapped]
        public IPAddress StartIP => new IPAddress(RawStartIP);

        [NotMapped]
        public IPAddress EndIP => new IPAddress(RawEndIP);

        public virtual DhcpSubnet DhcpSubnet { get; set; }
    }
}

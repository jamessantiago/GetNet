using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace getnet.core.Model.Entities
{
    public class DhcpSubnet
    {
        public int DhcpSubnetID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int RawIP { get; set; }
        public int RawSM { get; set; }
        [StringLength(100)]
        public string Comment { get; set; }
        public int RawServerIP { get; set; }

        [NotMapped]
        public IPAddress IP => new IPAddress(RawIP);

        [NotMapped]
        public IPAddress SM => new IPAddress(RawSM);

        [NotMapped]
        public IPAddress ServerIP => new IPAddress(RawServerIP);

        public virtual ICollection<DhcpPool> DhcpPools { get; set; }
    }
}

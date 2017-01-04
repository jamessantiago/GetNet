using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Net;

namespace getnet.core.Model.Entities
{
    public class Subnet
    {
        public int SubnetId { get; set; }

        [Required]
        public long RawSubnetIP { get; set; }

        [Required]
        public long RawSubnetSM { get; set; }

        public SubnetTypes Type { get; set; }

        [NotMapped]
        public IPAddress SubnetIP => new IPAddress(RawSubnetIP);

        [NotMapped]
        public IPAddress SubnetSM => new IPAddress(RawSubnetSM);

        [NotMapped]
        public IPNetwork IPNetwork => IPNetwork.Parse(SubnetIP, SubnetSM);
    }
}

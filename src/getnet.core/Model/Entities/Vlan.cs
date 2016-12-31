using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Net;

namespace getnet.core.Model.Entities
{
    public class Vlan
    {
        public int VlanId{ get; set; }

        [Required]
        public int VlanNumber { get; set; }

        [Required]
        public int RawVlanIP { get; set; }

        [Required]
        public int RawVlanSM { get; set; }

        [NotMapped]
        public IPAddress VlanIP => new IPAddress(RawVlanIP);

        [NotMapped]
        public IPAddress VlanSM => new IPAddress(RawVlanSM);

        [NotMapped]
        public IPNetwork IPNetwork => IPNetwork.Parse(VlanIP, VlanSM);

        public virtual NetworkDevice NetworkDevice { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}

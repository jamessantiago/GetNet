using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;

namespace getnet.core.Model.Entities
{
    public class Switch
    {
        [StringLength(100)]
        public string ChassisSerial { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        public string Hostname { get; set; }

        [NotMapped]
        public IPAddress ManagementIP => new IPAddress(RawManagementIP);

        [StringLength(50)]
        public string Model { get; set; }

        public int RawManagementIP { get; set; }
        public int SwitchId { get; set; }

        public bool IsSwitchBlade { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<RouterSwitchConnection> RouterSwitchConnections { get; set; }

        public virtual ICollection<SwitchSwitchConnection> SwitchSwitchConnections { get; set; }
    }
}
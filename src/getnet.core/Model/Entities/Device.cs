using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace getnet.core.Model.Entities
{
    public class Device
    {
        public int DeviceID { get; set; }
        public DeviceType Type { get; set; }

        [Required]
        public int RawIP { get; set; }

        [NotMapped]
        public IPAddress IP => new IPAddress(RawIP);

        [Required, StringLength(50)]
        public string MAC { get; set; }

        [StringLength(255)]
        public string Hostname { get; set; }

        public DateTime LastSeenOnline { get; set; }
        
        public DateTime DiscoveryDate { get; set; } 

        [StringLength(50)]
        public string SerialNumber { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Details { get; set; }

        [StringLength(100)]
        public string Port { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual Switch Switch { get; set; }
        public virtual Vlan Vlan { get; set; }
        public virtual Site Site { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
    }

    public class DeviceBuildItem : IModelBuildItem
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().HasIndex("MAC").IsUnique();
        }
    }
}

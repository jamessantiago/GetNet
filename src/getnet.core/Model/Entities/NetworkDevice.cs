using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace getnet.core.Model.Entities
{
    public class NetworkDevice
    {
        public int NetworkDeviceId { get; set; }

        [StringLength(100)]
        public string ChassisSerial { get; set; }
        
        [StringLength(500)]
        public string Details { get; set; }

        public string Hostname { get; set; }

        [NotMapped]
        public IPAddress ManagementIP => new IPAddress(RawManagementIP);

        [StringLength(50)]
        public string Model { get; set; }

        public NetworkCapabilities Capabilities { get; set; }

        [Required]
        public long RawManagementIP { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual ICollection<NetworkDeviceConnection> RemoteNetworkDeviceConnections { get; set; }

        public virtual ICollection<NetworkDeviceConnection> LocalNetworkDeviceConnections { get; set; }

        public virtual ICollection<Vlan> Vlans { get; set; }

        public virtual Site Site { get; set; }
    }

    public class NetworkDeviceBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NetworkDevice>().HasIndex("Model");
            modelBuilder.Entity<NetworkDevice>().HasIndex("RawManagementIP")
                .IsUnique();
            
        }
    }
}
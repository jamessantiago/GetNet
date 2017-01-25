using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace getnet.core.Model.Entities
{
    public class NetworkDeviceConnection
    {
        public int NetworkDeviceConnectionId { get; set; }

        public int NetworkDeviceId { get; set; }
        
        public int ConnectedNetworkDeviceId { get; set; }

        [StringLength(100)]
        public string DevicePort { get; set; }

        [StringLength(100)]
        public string ConnectedDevicePort { get; set; }

        [Required]
        public virtual NetworkDevice NetworkDevice { get; set; }

        [Required]
        public virtual NetworkDevice ConnectedNetworkDevice { get; set; }
    }

    public class NetworkDeviceNetworkDeviceConnectionBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NetworkDeviceConnection>()
                .HasKey(d => new { d.NetworkDeviceId, d.ConnectedNetworkDeviceId });

            modelBuilder.Entity<NetworkDeviceConnection>()                
                .HasOne(d => d.ConnectedNetworkDevice)
                .WithMany(d => d.LocalNetworkDeviceConnections)
                .HasPrincipalKey(d => d.NetworkDeviceId)
                .HasForeignKey(d => d.ConnectedNetworkDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NetworkDeviceConnection>()
                .HasOne(d => d.NetworkDevice)
                .WithMany(d => d.RemoteNetworkDeviceConnections)
                .HasPrincipalKey(d => d.NetworkDeviceId)
                .HasForeignKey(d => d.NetworkDeviceId);
        }
    }
}

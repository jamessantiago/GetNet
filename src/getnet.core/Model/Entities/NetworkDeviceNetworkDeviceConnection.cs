using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace getnet.core.Model.Entities
{
    public class NetworkDeviceNetworkDeviceConnection
    {
        public int NetworkDeviceId { get; set; }
        
        public int ConnectedNetworkDeviceId { get; set; }

        [StringLength(100)]
        public string DevicePort { get; set; }

        [StringLength(100)]
        public string ConnectedDevicePort { get; set; }

        public virtual NetworkDevice NetworkDevice { get; set; }
        public virtual NetworkDevice ConnectedNetworkDevice { get; set; }
    }

    public class NetworkDeviceNetworkDeviceConnectionBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NetworkDeviceNetworkDeviceConnection>()
                .HasKey(d => new { d.NetworkDeviceId, d.ConnectedNetworkDeviceId });

            modelBuilder.Entity<NetworkDeviceNetworkDeviceConnection>()                
                .HasOne(d => d.ConnectedNetworkDevice)
                .WithMany(d => d.LocalNetworkDeviceNetworkDeviceConnections)
                .HasPrincipalKey(d => d.NetworkDeviceId)
                .HasForeignKey(d => d.ConnectedNetworkDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NetworkDeviceNetworkDeviceConnection>()
                .HasOne(d => d.NetworkDevice)
                .WithMany(d => d.RemoteNetworkDeviceNetworkDeviceConnections)
                .HasPrincipalKey(d => d.NetworkDeviceId)
                .HasForeignKey(d => d.NetworkDeviceId);
        }
    }
}

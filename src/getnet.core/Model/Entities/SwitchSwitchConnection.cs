using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace getnet.core.Model.Entities
{
    public class SwitchSwitchConnection
    {
        public int SwitchId { get; set; }
        
        public int ConnectedSwitchId { get; set; }

        [StringLength(100)]
        public string SwitchPort { get; set; }

        [StringLength(100)]
        public string ConnectedSwitchPort { get; set; }

        public virtual Switch Switch { get; set; }
        public virtual Switch ConnectedSwitch { get; set; }
    }

    public class SwitchSwitchConnectionBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SwitchSwitchConnection>()
                .HasKey(d => new { d.SwitchId, d.ConnectedSwitchId });

            modelBuilder.Entity<SwitchSwitchConnection>()
               .HasOne(d => d.Switch)
               .WithMany(d => d.InSwitchSwitchConnections)
               .HasPrincipalKey(d => d.SwitchId)
               .HasForeignKey(d => d.SwitchId);

            modelBuilder.Entity<SwitchSwitchConnection>()
                .HasOne(d => d.ConnectedSwitch)
                .WithMany(d => d.OutSwitchSwitchConnections)
                .HasPrincipalKey(d => d.SwitchId)
                .HasForeignKey(d => d.ConnectedSwitchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace getnet.core.Model.Entities
{
    public class SwitchSwitchConnection
    {
        [Key, Column(Order = 0)]
        public int SwitchId { get; set; }

        [Key, Column(Order = 1)]
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
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SwitchSwitchConnection>()
               .HasOne(d => d.Switch)
               .WithMany(d => d.SwitchSwitchConnections)
               .HasForeignKey(d => d.SwitchId);

            modelBuilder.Entity<SwitchSwitchConnection>()
                .HasOne(d => d.ConnectedSwitch)
                .WithMany(d => d.SwitchSwitchConnections)
                .HasForeignKey(d => d.ConnectedSwitchId);
        }
    }
}

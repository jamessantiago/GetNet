using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace getnet.core.Model.Entities
{
    public class RouterRouterConnection
    {
        [Key, Column(Order = 0)]
        public int RouterId { get; set; }

        [Key, Column(Order = 1)]
        public int ConnectedRouterId { get; set; }

        [StringLength(100)]
        public string RouterPort { get; set; }

        [StringLength(100)]
        public string ConnectedRouterPort { get; set; }

        public virtual Router Router { get; set; }
        public virtual Router ConnectedRouter { get; set; }
    }

    public class RouterRouterConnectionBuildItem : IModelBuildItem
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouterRouterConnection>()
                .HasOne(d => d.Router)
                .WithMany(d => d.RouterRouterConnections)
                .HasForeignKey(d => d.RouterId);

            modelBuilder.Entity<RouterRouterConnection>()
                .HasOne(d => d.ConnectedRouter)
                .WithMany(d => d.RouterRouterConnections)
                .HasForeignKey(d => d.ConnectedRouterId);
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace getnet.core.Model.Entities
{
    public class RouterRouterConnection
    {
        public int RouterId { get; set; }
        
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
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouterRouterConnection>()
                .HasKey(d => new { d.RouterId, d.ConnectedRouterId });

            modelBuilder.Entity<RouterRouterConnection>()                
                .HasOne(d => d.ConnectedRouter)
                .WithMany(d => d.InRouterRouterConnections)
                .HasPrincipalKey(d => d.RouterId)
                .HasForeignKey(d => d.ConnectedRouterId);

            modelBuilder.Entity<RouterRouterConnection>()
                .HasOne(d => d.Router)
                .WithMany(d => d.OutRouterRouterConnections)
                .HasPrincipalKey(d => d.RouterId)
                .HasForeignKey(d => d.RouterId);
        }
    }
}

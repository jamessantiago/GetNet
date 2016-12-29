using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace getnet.core.Model.Entities
{
    public class Router
    {
        public int RouterId { get; set; }

        [StringLength(100)]
        public string ChassisSerial { get; set; }
        
        [StringLength(500)]
        public string Details { get; set; }

        public string Hostname { get; set; }

        [NotMapped]
        public IPAddress ManagementIP => new IPAddress(RawManagementIP);

        [StringLength(50)]
        public string Model { get; set; }

        [Required]
        public int RawManagementIP { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<RouterSwitchConnection> RouterSwitchConnections { get; set; }

        public virtual ICollection<RouterRouterConnection> OutRouterRouterConnections { get; set; }

        public virtual ICollection<RouterRouterConnection> InRouterRouterConnections { get; set; }

        public virtual ICollection<Vlan> Vlans { get; set; }
    }

    public class RouterBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Router>().HasIndex("Model");
            
        }
    }
}
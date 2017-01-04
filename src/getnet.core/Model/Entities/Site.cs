﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace getnet.core.Model.Entities
{
    public class Site
    {
        public int SiteId { get; set; }

        [StringLength(100)]
        public string Building { get; set; }
        
        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Owner { get; set; }

        [StringLength(500)]
        public string Details { get; set; }
        
        public SiteStatus Status { get; set; }

        public virtual Location Location { get; set; }
        public virtual Priority Priority { get; set; }

        public virtual ICollection<NetworkDevice> NetworkDevices { get; set; }
        public virtual ICollection<PointOfContact> PointOfContacts { get; set; }
        public virtual ICollection<Diagram> Diagrams { get; set; }
        public virtual ICollection<AlertRule> AlertRules { get; set; }
        public virtual ICollection<HotPath> HotPaths { get; set; }
        public virtual ICollection<Vlan> Vlans { get; set; }
        public virtual ICollection<Subnet> Subnets { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
    }

    public class SiteBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>().HasIndex("Status");
            
        }
    }
}
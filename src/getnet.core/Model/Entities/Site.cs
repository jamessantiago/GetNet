using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace getnet.core.Model.Entities
{
    public class Site
    {
        [StringLength(100)]
        public string Building { get; set; }
        
        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Owner { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        public int SiteId { get; set; }
        public SiteStatus Status { get; set; }

        public virtual Location Location { get; set; }
        public virtual Priority Priority { get; set; }

        public virtual List<Router> Routers { get; set; }
        public virtual List<Switch> Switches { get; set; }
        public virtual List<PointOfContact> PointOfContacts { get; set; }
        public virtual List<Diagram> Diagrams { get; set; }
    }

    public class SiteBuildItem : IModelBuildItem
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>().HasIndex("Status");
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace getnet.core.Model.Entities
{
    public class Location
    {
        public int LocationId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }

    public class LocationBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>().HasIndex("Name").IsUnique();

        }
    }
}

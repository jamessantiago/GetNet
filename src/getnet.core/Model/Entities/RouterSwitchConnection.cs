using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace getnet.core.Model.Entities
{
    public class RouterSwitchConnection
    {
        public int RouterId { get; set; }
        
        public int SwitchId { get; set; }

        [StringLength(100)]
        public string RouterPort { get; set; }

        [StringLength(100)]
        public string SwitchPort { get; set; }

        public virtual Router Router { get; set; }
        public virtual Switch Switch { get; set; }
    }

    public class RouterSwitchConnectionBuildItem : IModelBuildItem
    {
        public void Build(ref ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RouterSwitchConnection>()
                .HasKey("RouterId", "SwitchId");
        }
    }
}

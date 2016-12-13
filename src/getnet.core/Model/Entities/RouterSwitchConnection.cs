using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata;

namespace getnet.core.Model.Entities
{
    public class RouterSwitchConnection
    {
        [Key, Column(Order = 0)]
        public int RouterId { get; set; }

        [Key, Column (Order = 1)]
        public int SwitchId { get; set; }

        [StringLength(100)]
        public string RouterPort { get; set; }

        [StringLength(100)]
        public string SwitchPort { get; set; }

        public virtual Router Router { get; set; }
        public virtual Switch Switch { get; set; }
    }
}

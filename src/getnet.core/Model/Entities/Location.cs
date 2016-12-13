using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace getnet.core.Model.Entities
{
    public class Location
    {
        public int LocationId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}

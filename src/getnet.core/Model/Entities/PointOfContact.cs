using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace getnet.core.Model.Entities
{
    public class PointOfContact
    {
        public int PointOfContactId { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        [StringLength(15)]
        public string AltPhone { get; set; }

        [StringLength(250)]
        public string Organization { get; set; }

        public virtual Site Site { get; set; }
        public virtual Tenant Tenant { get; set; }
        
    }
}

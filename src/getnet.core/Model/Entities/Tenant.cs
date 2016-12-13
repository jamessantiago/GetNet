using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace getnet.core.Model.Entities
{
    public class Tenant
    {
        public int TenantId { get; set; }

        [StringLength(3)]
        public string TenantCode { get; set; }
        
    }
}

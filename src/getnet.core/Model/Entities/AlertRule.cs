using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using NLog;

namespace getnet.core.Model.Entities
{
    public class AlertRule
    {
        public int AlertRuleId { get; set; }
        
        [StringLength(200)]
        public string Type { get; set; }

        [StringLength(50)]
        public string LogLevel { get; set; }
        
        public virtual UserProfile User { get; set; }
        public virtual Site Site { get; set; }
    }
}

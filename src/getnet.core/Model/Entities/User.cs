using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace getnet.core.Model.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, StringLength(200)]
        public string Email { get; set; }

        public virtual ICollection<AlertRule> AlertRules { get; set; }
    }
}

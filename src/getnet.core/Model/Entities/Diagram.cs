﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace getnet.core.Model.Entities
{
    public class Diagram
    {
        public int DiagramID { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string FilePath { get; set; }

        public virtual Site Site { get; set; }
    }
}

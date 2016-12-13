﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace getnet.core.Model.Entities
{
    [Table("DeviceHistories")]
    public class DeviceHistory
    {
        public int DeviceHistoryId { get; set; }

        [StringLength(50)]
        public string MAC { get; set; }

        public DeviceType Type { get; set; }

        [StringLength(255)]
        public string Hostname { get; set; }

        public DateTime LastSeenOnline { get; set; }
        public DateTime DiscoveryDate { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Details { get; set; }

        [StringLength(100)]
        public string Port { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}

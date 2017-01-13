﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.Model.Entities
{
    public enum ScheduleType
    {
        HotpathCheck,
        FullSiteDiscovery,
        DeviceDiscovery,
        DhcpDiscovery,
        SitesAndSubnetsDiscovery
    }
}

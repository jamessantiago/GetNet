using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.Model.Entities
{
    [Flags]
    public enum NetworkCapabilities
    {
        Device = 0,
        Router = 1 << 1,
        Switch = 1 << 2
    }
}

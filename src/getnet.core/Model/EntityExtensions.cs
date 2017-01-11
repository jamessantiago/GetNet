using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model.Entities;

namespace getnet.core.Model
{
    public static class EntityExtensions
    {
        public static bool AllOnline(this ICollection<HotPath> paths)
        {
            return paths.All(d => d.Status == HotPathStatus.Online);
        }

        public static bool IsDegraded(this ICollection<HotPath> paths)
        {
            return paths.Any(d => d.Status == HotPathStatus.Online) && paths.Any(d => d.Status == HotPathStatus.Offline);
        }

        public static bool AllOffline(this ICollection<HotPath> paths)
        {
            return paths.All(d => d.Status == HotPathStatus.Offline);
        }
    }
}

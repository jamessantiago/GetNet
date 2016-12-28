using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet
{
    public static class Util
    {
        public static bool AllSame(params int[] list)
        {
            return (list as IEnumerable<int>).AllSame();
        }
    }
}

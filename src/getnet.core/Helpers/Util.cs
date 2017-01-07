using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace getnet
{
    public static class Util
    {
        public static bool AllSame(params int[] list)
        {
            return (list as IEnumerable<int>).AllSame();
        }

        public static int CountIPs(IPAddress StartIP, IPAddress EndIP)
        {
            byte[] endbytes = EndIP.GetAddressBytes();
            byte[] startbytes = StartIP.GetAddressBytes();

            int[] differencevals = new int[4];

            for (int i = 0; i < 4; i++)
            {
                if (i - 1 >= 0 && differencevals[i - 1] > 0)
                {
                    int hosts = byte.MaxValue * differencevals[i - 1];
                    differencevals[i - 1] = 0;
                    differencevals[i] = hosts + (endbytes[i] - startbytes[i]);
                }
                else
                {
                    differencevals[i] = (endbytes[i] - startbytes[i]);
                }
            }

            return differencevals[3];
        }
    }
}

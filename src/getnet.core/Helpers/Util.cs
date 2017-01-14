using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

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

        public static async Task<bool> PingHost(string host, int attempts, int timeout)
        {
            using (Ping ping = new Ping())
            {
                PingReply pingreply;

                for (int i = 0; i < attempts; i++)
                {
                    try
                    {
                        pingreply = await ping.SendPingAsync(host, timeout);
                        if (pingreply != null && pingreply.Status == IPStatus.Success)
                            return true;
                    }
                    catch
                    {
                        //well
                    }
                    //well
                }
            }
            //well
            return false;
        }

        public static bool Ping(this IPAddress ip)
        {
            return PingHost(ip.ToString(), 1, 1000).Result;
        }
    }
}

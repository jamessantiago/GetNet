
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace Dwo
{
    /// <summary>
    /// Provides an IP address for the DHCP SMO library
    /// </summary>
    public class DhcpIpAddress
    {
        /// <summary>
        /// Sets Ip string to 0.0.0.0
        /// </summary>
        public DhcpIpAddress() { this.ip = IPAddress.Any.ToString(); }
        /// <summary>
        /// Sets Ip to IpString
        /// </summary>
        /// <param name="IpString">IP address in string form</param>
        public DhcpIpAddress(String IpString) { this.Ip = IpString; }
        /// <summary>
        /// Sets Ip from an UInt representation
        /// </summary>
        /// <param name="IpUInt">IP address in uint form</param>
        public DhcpIpAddress(UInt32 IpUInt) { this.Ip = DhcpIpAddress.UInt2Str(IpUInt); }

        String ip;
        /// <summary>
        /// IP address in string form w.x.y.z
        /// </summary>
        public String Ip
        {
            get
            {
                return ip;
            }
            set
            {
                IPAddress outIp;
                if (!IPAddress.TryParse(value, out outIp))
                {
                    ip = IPAddress.Any.ToString();
                    throw new FormatException("IpAddress invalid format");
                }
                ip = value;
            }
        }

        /// <summary>
        /// IP address in string form w.x.y.z
        /// </summary>
        /// <returns>IP address in string form w.x.y.z</returns>
        public override String ToString()
        {
            return Ip;
        }

        /// <summary>
        /// Gets this Ip in uint form
        /// </summary>
        /// <returns>IP address in uint form</returns>
        public UInt32 GetUIntAddress()
        {
            return DhcpIpAddress.Str2UInt(this.ip);
        }

        /// <summary>
        /// Converts a string form IP address to its uint form
        /// </summary>
        /// <param name="IpString">String IP to convert</param>
        /// <returns>IP address converted to unit form</returns>
        public static UInt32 Str2UInt(String IpString)
        {
            IPAddress ip = IPAddress.Parse(IpString);

            byte[] ipBytes = ip.GetAddressBytes();
            
            UInt32 ipUInt = (UInt32)ipBytes[0] << 24;
            ipUInt += (UInt32)ipBytes[1] << 16;
            ipUInt += (UInt32)ipBytes[2] << 8;
            ipUInt += (UInt32)ipBytes[3];
            
            return ipUInt;
        }

        /// <summary>
        /// Converts a uint form IP address to its string form
        /// </summary>
        /// <param name="IpUInt">Uint IP to convert</param>
        /// <returns>IP address converted to string form</returns>
        public static String UInt2Str(UInt32 IpUInt)
        {
            IPAddress ip = new IPAddress(IpUInt);
            string[] strIp = ip.ToString().Split('.');

            return strIp[3] + "." + strIp[2] + "." + strIp[1] + "." + strIp[0];
        }
    }
}

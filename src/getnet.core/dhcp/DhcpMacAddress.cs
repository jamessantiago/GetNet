
using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace Dwo
{
    /// <summary>
    /// Provides a MAC address for the DHCP SMO library
    /// </summary>
    /// <remarks>Accepted formats: 00:01:02:0a:0B:0c, 00-01-02-0a-0B-0c, or 0001020a0B0c</remarks>
    public class DhcpMacAddress
    {

        /// <summary>
        /// Sets Mac to zeros
        /// </summary>
        public DhcpMacAddress() { this.mac = "000000000000"; }
        /// <summary>
        /// Sets Mac to MacString
        /// </summary>
        /// <param name="MacString">String form of Mac address</param>
        /// <remarks>Accepted formats: 00:01:02:0a:0B:0c, 00-01-02-0a-0B-0c, or 0001020a0B0c</remarks>
        public DhcpMacAddress(String MacString) { this.Mac = MacString; }
        /// <summary>
        /// Sets Mac to MacBytes
        /// </summary>
        /// <param name="MacBytes">Array of bytes</param>
        public DhcpMacAddress(Byte[] MacBytes)
        {
            PhysicalAddress InMac = new PhysicalAddress(MacBytes);
            this.mac = InMac.ToString();
        }

        private String mac;
        /// <summary>
        /// Mac address in string form 0001020A0B0C
        /// </summary>
        public String Mac
        {
            get { return this.mac; }
            set
            {
                String InMac = value.ToUpper();

                InMac = InMac.Replace(':', '-');

                this.mac = PhysicalAddress.Parse(InMac).ToString();
            }
        }

        /// <summary>
        /// Mac address in string form
        /// </summary>
        /// <returns>Mac address in string form</returns>
        public override String ToString()
        {
            return Mac;
        }

        /// <summary>
        /// Gets Mac address in byte array form
        /// </summary>
        /// <returns>Byte array form of Mac</returns>
        public Byte[] GetByteArray()
        {
            return PhysicalAddress.Parse(this.mac).GetAddressBytes();
        }
    }
}

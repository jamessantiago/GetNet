
using System;
using System.Collections.Generic;
using System.Text;


namespace Dwo
{
    /// <summary>
    /// Define a DHCP reservation.  A wrapper class of information combined from a client lease and IP reservation object
    /// </summary>
    public class DhcpReservation
    {
        /// <summary>
        /// IP addres of reservation
        /// </summary>
        public DhcpIpAddress ReservedIp;
        /// <summary>
        /// MAC address of reservation
        /// </summary>
        public DhcpMacAddress ReservedMac;
        /// <summary>
        /// Reservation client type (DHCP, BOOTP, or Both)
        /// </summary>
        public DhcpSubnetClientType bAllowedClientTypes;
        /// <summary>
        /// Name of reservation (will be replaced by hostname client get lease)
        /// </summary>
        public String Name;
        /// <summary>
        /// Description of reservation
        /// </summary>
        public String Comment;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpReservation() {}
    }
}

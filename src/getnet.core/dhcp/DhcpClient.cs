using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Dwo.Interop;

namespace Dwo
{
    /// <summary>
    /// DhcpClient defines a client lease in the DHCP server database
    /// </summary>
    public class DhcpClient
    {
        /// <summary>
        /// IP address of client
        /// </summary>
        public DhcpIpAddress IpAddress;
        /// <summary>
        /// Subnet mask of client
        /// </summary>
        public DhcpIpAddress SubnetMask;
        /// <summary>
        /// MAC address of client
        /// </summary>
        public DhcpMacAddress MacAddress;
        /// <summary>
        /// Hostname of client
        /// </summary>
        public String Name;
        /// <summary>
        /// Description string for client
        /// </summary>
        public String Comment;
        /// <summary>
        /// Date and time client lease will expire
        /// </summary>
        public DateTime LeaseExpires;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpClient() { }

        /// <summary>
        /// Create this object on DHCP server
        /// </summary>
        /// <remarks>IpAddress, SubnetMask, MacAddress, and LeaseExpires must be set before calling Create()</remarks>
        /// <param name="server">Server IP</param>
        public void Create(DhcpIpAddress server)
        {
            using (MemManager mem = new MemManager())
            {
                DHCP_CLIENT_INFO info = this.ConvertToNative(mem);

                uint Response = NativeMethods.DhcpCreateClientInfo(server.ToString(), ref info);

                if (Response != 0)
                    throw new DhcpException(Response);
            }
            
        }

        /// <summary>
        /// Update this object on DHCP server
        /// </summary>
        /// <remarks>IpAddress, SubnetMask, MacAddress, and LeaseExpires must be set before calling Update()</remarks>
        /// <param name="server">Server IP</param>
        public void Update(DhcpIpAddress server)
        {
            using (MemManager mem = new MemManager())
            {
                DHCP_CLIENT_INFO info = this.ConvertToNative(mem);

                uint Response = NativeMethods.DhcpSetClientInfo(server.ToString(), ref info);

                if (Response != 0)
                    throw new DhcpException(Response);
            }
        }

        /// <summary>
        /// Delete client lease on DHCP server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <param name="term">Search term describing client to delete (deletes first match)</param>
        public static void Delete(DhcpIpAddress server, DhcpSearchInfo term)
        {
            using (MemManager mem = new MemManager())
            {
                DHCP_SEARCH_INFO sinfo = term.ConvertToNative(mem);

                uint Response = NativeMethods.DhcpDeleteClientInfo(server.ToString(), ref sinfo);

                if (Response != 0)
                    throw new DhcpException(Response);
            }
        }

        /// <summary>
        /// Get client lease from DHCP server
        /// </summary>
        /// <param name="server">Server to query</param>
        /// <param name="term">Search term used to find client</param>
        /// <returns>Returns first match based on term</returns>
        public static DhcpClient Get(DhcpIpAddress server, DhcpSearchInfo term)
        {
            IntPtr iPtr;
            DhcpClient client = new DhcpClient();
            using (MemManager mem = new MemManager())
            {
                DHCP_SEARCH_INFO sinfo = term.ConvertToNative(mem);

                uint Response = NativeMethods.DhcpGetClientInfo(server.ToString(), ref sinfo, out iPtr);

                if (Response != 0)
                {
                    if (iPtr != IntPtr.Zero)
                        NativeMethods.DhcpRpcFreeMemory(iPtr);
                    throw new DhcpException(Response);
                }

                DHCP_CLIENT_INFO cinfo = (DHCP_CLIENT_INFO)Marshal.PtrToStructure<DHCP_CLIENT_INFO>(iPtr);
                client.IpAddress = new DhcpIpAddress(cinfo.ClientIpAddress);
                client.SubnetMask = new DhcpIpAddress(cinfo.SubnetMask);
                client.Name = cinfo.ClientName;
                client.Comment = cinfo.ClientComment;
                client.LeaseExpires = cinfo.ClientLeaseExpires.Convert();

                byte[] mac = new byte[cinfo.ClientHardwareAddress.DataLength];
                Marshal.Copy(cinfo.ClientHardwareAddress.Data, mac, 0, mac.Length);
                client.MacAddress = new DhcpMacAddress(mac);

                if (iPtr != IntPtr.Zero)
                    NativeMethods.DhcpRpcFreeMemory(iPtr);
            }
            return client;
        }

        /// <summary>
        /// Enumeration all client leases found in subnet on DHCP server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <param name="subnet">Subnet IP to enumerate</param>
        /// <returns>A list of client leases or an empty list</returns>
        public static List<DhcpClient> EnumAll(DhcpIpAddress server, DhcpIpAddress subnet)
        {
            UInt32 Response = 0, ResumeHandle = 0;
            UInt32 nRead = 0, nTotal = 0;
            IntPtr retPtr, iPtr;
            List<DhcpClient> clients = new List<DhcpClient>();
            DHCP_CLIENT_INFO nativeClient;
            DHCP_CLIENT_INFO_ARRAY nativeArray;

            for ( ; ; )
            {
                Response = NativeMethods.DhcpEnumSubnetClients(server.ToString(), subnet.GetUIntAddress(),
                    ref ResumeHandle, 65536, out retPtr, out nRead, out nTotal);

                //ERROR_NO_MORE_ITEMS
                if (Response == 259)
                    break;

                //ERROR_MORE_DATA = 234
                if (Response != 0 && Response != 234 || retPtr == IntPtr.Zero)
                {
                    if (retPtr != IntPtr.Zero)
                        NativeMethods.DhcpRpcFreeMemory(retPtr);
                    throw new DhcpException(Response);
                }

                //work
                nativeArray = (DHCP_CLIENT_INFO_ARRAY)Marshal.PtrToStructure<DHCP_CLIENT_INFO_ARRAY>(retPtr);
                for (int i = 0; i < nativeArray.NumElements; ++i)
                {
                    iPtr = Marshal.ReadIntPtr(nativeArray.Elements, (i * Marshal.SizeOf<IntPtr>()));
                    nativeClient = (DHCP_CLIENT_INFO)Marshal.PtrToStructure<DHCP_CLIENT_INFO>(iPtr);
                    clients.Add(DhcpClient.ConvertFromNative(nativeClient));
                }

                //free on last successful call
                if (Response == 0)
                {
                    NativeMethods.DhcpRpcFreeMemory(retPtr);
                    break;
                }
            }
            return clients;
        }

        /// <summary>
        /// Converts this client object to its native representation
        /// </summary>
        /// <param name="Mem">Used to track memory allocations</param>
        /// <returns>Native form of a client lease</returns>
        internal DHCP_CLIENT_INFO ConvertToNative(MemManager Mem)
        {
            if (this.IpAddress == null || this.SubnetMask == null || this.MacAddress == null
                || this.LeaseExpires == null)
                throw new InvalidOperationException("Client invalid");

            DHCP_CLIENT_INFO info = new DHCP_CLIENT_INFO();
            info.ClientIpAddress = this.IpAddress.GetUIntAddress();
            info.SubnetMask = this.SubnetMask.GetUIntAddress();
            info.ClientName = this.Name;
            info.ClientComment = this.Comment;

            Byte[] mac = this.MacAddress.GetByteArray();
            DHCP_CLIENT_UID uid = new DHCP_CLIENT_UID();
            uid.DataLength = (uint)mac.Length;
            uid.Data = Mem.AllocByteArray(this.MacAddress.GetByteArray());
            info.ClientHardwareAddress = uid;

            info.ClientLeaseExpires = new DATE_TIME(this.LeaseExpires);
           
            return info;
        }

        /// <summary>
        /// Converts a native client lease to an SMO client lease
        /// </summary>
        /// <param name="nativeClient">Native client to convert</param>
        /// <returns>SMO client lease</returns>
        internal static DhcpClient ConvertFromNative(DHCP_CLIENT_INFO nativeClient)
        {
            DhcpClient client = new DhcpClient();
            client.IpAddress = new DhcpIpAddress(nativeClient.ClientIpAddress);
            client.SubnetMask = new DhcpIpAddress(nativeClient.SubnetMask);
            client.Name = nativeClient.ClientName;
            client.Comment = nativeClient.ClientComment;
            client.LeaseExpires = nativeClient.ClientLeaseExpires.Convert();
            Console.WriteLine(client.LeaseExpires.ToString());

            byte[] mac = new byte[nativeClient.ClientHardwareAddress.DataLength];
            Marshal.Copy(nativeClient.ClientHardwareAddress.Data, mac, 0, mac.Length);
            client.MacAddress = new DhcpMacAddress(mac);
            return client;
        }
    }

    /// <summary>
    /// Defines search terms used by DhcpClient methods searching for client leases
    /// </summary>
    public class DhcpSearchInfo
    {
        /// <summary>
        /// Search by type (IpAddress, HardwareAddress, or Name)
        /// </summary>
        public DhcpSearchInfoType Type;
        /// <summary>
        /// MAC address of client
        /// </summary>
        public DhcpMacAddress MacAddress;
        /// <summary>
        /// IP address of client
        /// </summary>
        public DhcpIpAddress IpAddress;
        /// <summary>
        /// Hostname of client
        /// </summary>
        public String Name;

        /// <summary>
        /// Converts this object to its native representation
        /// </summary>
        /// <param name="Mem">Used to track memory allocations</param>
        /// <returns>A native search info structure</returns>
        internal DHCP_SEARCH_INFO ConvertToNative(MemManager Mem)
        {
            DHCP_SEARCH_INFO info = new DHCP_SEARCH_INFO();
            switch (this.Type)
            {
                case DhcpSearchInfoType.IpAddress:
                    if (this.IpAddress == null)
                        throw new InvalidOperationException("SearchInfo invalid");

                    info.SearchType = DHCP_SEARCH_INFO_TYPE.DhcpClientIpAddress;
                    info.ClientIpAddress = this.IpAddress.GetUIntAddress();
                    return info;
                case DhcpSearchInfoType.HardwareAddress:
                    if (this.MacAddress == null)
                        throw new InvalidOperationException("SearchInfo invalid");
                    info.SearchType = DHCP_SEARCH_INFO_TYPE.DhcpClientHardwareAddress;

                    Byte[] mac = this.MacAddress.GetByteArray();
                    DHCP_CLIENT_UID uid = new DHCP_CLIENT_UID();
                    uid.DataLength = (uint)mac.Length;
                    uid.Data = Mem.AllocByteArray(this.MacAddress.GetByteArray());
                    info.ClientHardwareAddress = uid;

                    return info;
                case DhcpSearchInfoType.Name:
                    if (this.Name == null)
                        throw new InvalidOperationException("SearchInfo invalid");
                    info.SearchType = DHCP_SEARCH_INFO_TYPE.DhcpClientName;
                    info.ClientName = Mem.AllocString(this.Name);
                    return info;
            }
            return info;
        }
    }

    /// <summary>
    /// Search by type
    /// </summary>
    public enum DhcpSearchInfoType
    {
        /// <summary>
        /// Search by IP address
        /// </summary>
        IpAddress,
        /// <summary>
        /// Search by MAC address
        /// </summary>
        HardwareAddress,
        /// <summary>
        /// Search by client hostname
        /// </summary>
        Name
    }
}

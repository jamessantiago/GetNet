
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Dwo.Interop;

namespace Dwo
{
    /// <summary>
    /// Defines a DHCP subnet
    /// </summary>
    public class DhcpSubnet
    {
        /// <summary>
        /// Subnet network IP address
        /// </summary>
        public DhcpIpAddress Address;
        /// <summary>
        /// Subnet network mask address
        /// </summary>
        public DhcpIpAddress Mask;
        /// <summary>
        /// Subnet name value
        /// </summary>
        public String Name;
        /// <summary>
        /// Subnet description value
        /// </summary>
        public String Comment;
        /// <summary>
        /// Subnet state (Enable, Disabled,...)
        /// </summary>
        public DhcpSubnetState State;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpSubnet() { this.State = DhcpSubnetState.Disabled; }

        /// <summary>
        /// Creates a subnet on server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <remarks>Address, Mask, and Name fields must be set before calling Create()</remarks>
        public void Create(DhcpIpAddress server)
        {
            DHCP_SUBNET_INFO info = this.ConvertToNative();

            uint Response = NativeMethods.DhcpCreateSubnet(server.ToString(), this.Address.GetUIntAddress(), ref info);

            if (Response != 0)
                throw new DhcpException(Response);
        }

        /// <summary>
        /// Deletes a subnet on server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <param name="force">Force deletion of subnet child elements</param>
        /// <remarks>Address field must be set before calling Delete()</remarks>
        public void Delete(DhcpIpAddress server, bool force)
        {
            if (this.Address == null)
                throw new InvalidOperationException("Subnet invalid");
            uint Response = NativeMethods.DhcpDeleteSubnet(server.ToString(), this.Address.GetUIntAddress(), (force ? DHCP_FORCE_FLAG.DhcpFullForce : DHCP_FORCE_FLAG.DhcpNoForce));

            if (Response != 0)
                throw new DhcpException(Response);
        }

        /// <summary>
        /// Gets a subnet on server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <remarks>Address field must be set before calling Get().  After call object fields will be set based on address</remarks>
        public void Get(DhcpIpAddress server)
        {
            if (this.Address == null)
                throw new InvalidOperationException("Subnet invalid");

            IntPtr iPtr = IntPtr.Zero;
            UInt32 Response = 0;
            DHCP_SUBNET_INFO info = new DHCP_SUBNET_INFO();
            try
            {
                Response = NativeMethods.DhcpGetSubnetInfo(server.ToString(), this.Address.GetUIntAddress(), out iPtr);

                info = (DHCP_SUBNET_INFO)Marshal.PtrToStructure<DHCP_SUBNET_INFO>(iPtr);
            }
            finally
            {
                if (iPtr != IntPtr.Zero)
                    NativeMethods.DhcpRpcFreeMemory(iPtr);
            }

            this.Address = new DhcpIpAddress(info.SubnetAddress);
            this.Mask = new DhcpIpAddress(info.SubnetMask);
            this.Name = info.SubnetName;
            this.Comment = info.SubnetComment;
            this.State = (DhcpSubnetState)info.SubnetState;

            if (Response != 0)
                throw new DhcpException(Response);
            
        }

        /// <summary>
        /// Update subnet on server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <remarks>Address, Mask, and Name fields must be set before calling Update()</remarks>
        public void Update(DhcpIpAddress server)
        {
            DHCP_SUBNET_INFO info = this.ConvertToNative();

            uint Response = NativeMethods.DhcpSetSubnetInfo(server.ToString(), this.Address.GetUIntAddress(), ref info);

            if (Response != 0)
                throw new DhcpException(Response);
        }


        /// <summary>
        /// Enumerate all subnets on server
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <returns>A list of DhcpSubnets on server or an empty list</returns>
        public static List<DhcpSubnet> EnumAll(DhcpIpAddress server)
        {
            UInt32 Response = 0, ResumeHandle = 0;
            UInt32 nRead = 0, nTotal = 0;
            IntPtr retPtr;
            List<DhcpSubnet> subnets = new List<DhcpSubnet>();
            DhcpSubnet current;

            for( ; ; )
            {
                Response = NativeMethods.DhcpEnumSubnets(server.ToString(), ref ResumeHandle, 1024, out retPtr,
                    out nRead, out nTotal);

                //ERROR_NO_MORE_ITEMS
                if (Response == 259)
                    break;

                if (Response != 0)
                    throw new DhcpException(Response);

                //work
                DHCP_IP_ARRAY NativeIps = (DHCP_IP_ARRAY)Marshal.PtrToStructure(retPtr, typeof(DHCP_IP_ARRAY));

                for (int i = 0; i < NativeIps.NumElements; ++i)
                {
                    current = new DhcpSubnet();
                    current.Address = new DhcpIpAddress((UInt32)Marshal.ReadInt32(NativeIps.Elements, i*Marshal.SizeOf(typeof(UInt32))));
                    current.Get(server);
                    subnets.Add(current);
                }   
            }

            //free on last successful call
            if (Response == 0)
                NativeMethods.DhcpRpcFreeMemory(retPtr);

            return subnets;
        }

        /// <summary>
        /// Converts this object to its native form
        /// </summary>
        /// <returns>A native subnet info</returns>
        private DHCP_SUBNET_INFO ConvertToNative()
        {
            if (this.Address == null || this.Mask == null || String.IsNullOrEmpty(this.Name))
                throw new InvalidOperationException("Subnet invalid");

            DHCP_SUBNET_INFO nativeSubnet = new DHCP_SUBNET_INFO();
            nativeSubnet.SubnetAddress = this.Address.GetUIntAddress();
            nativeSubnet.SubnetMask = this.Mask.GetUIntAddress();
            nativeSubnet.SubnetName = this.Name;
            nativeSubnet.SubnetComment = this.Comment;
            nativeSubnet.SubnetState = (DHCP_SUBNET_STATE)this.State;
            return nativeSubnet;
        }

        /// <summary>
        /// Converts a native subnet info to its SMO form
        /// </summary>
        /// <param name="info">A native subnet info</param>
        /// <returns>The SMO representation</returns>
        private static DhcpSubnet ConvertFromNative(DHCP_SUBNET_INFO info)
        {
            DhcpSubnet subnet = new DhcpSubnet();
            subnet.Address = new DhcpIpAddress(info.SubnetAddress);
            subnet.Mask = new DhcpIpAddress(info.SubnetMask);
            subnet.Name = info.SubnetName;
            subnet.Comment = info.SubnetComment;
            subnet.State = (DhcpSubnetState)info.SubnetState;
            return subnet;
        }
    }

    /// <summary>
    /// DHCP Subnet state
    /// </summary>
    public enum DhcpSubnetState
    {
        /// <summary>
        /// Subnet is enabled
        /// </summary>
        Enabled,
        /// <summary>
        /// Subnet is disabled
        /// </summary>
        Disabled,
        /// <summary>
        /// Subnet is enabled and the gateway of clients
        /// </summary>
        EnabledSwitched,
        /// <summary>
        /// Subnet is enable and the gateway of clients
        /// </summary>
        DisabledSwitched,
        /// <summary>
        /// Subnet is in an invalid state
        /// </summary>
        InvalidState
    }
}

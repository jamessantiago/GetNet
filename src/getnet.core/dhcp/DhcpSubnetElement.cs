
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Dwo.Interop;

namespace Dwo
{
    /// <summary>
    /// Defines an element that describe feature of a DHCP subnet
    /// </summary>
    public class DhcpSubnetElement
    {
        /// <summary>
        /// Subnet network IP address.
        /// </summary>
        public DhcpIpAddress SubnetAddress;
        /// <summary>
        /// Element data that describes the feature.
        /// </summary>
        /// <remarks>Data can be one of three types: <see cref="DhcpIpReservation"/>, <see cref="DhcpIpRange"/>, or <see cref="BootpIpRange"/></remarks>
        public DhcpSubnetElementData Data;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpSubnetElement() { }

        /// <summary>
        /// Add this element to server's subnet (defined by SubnetAddress)
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <remarks>SubnetAddress and Data must be defined before calling Add()</remarks>
        public void Add(DhcpIpAddress server)
        {
            if(this.SubnetAddress == null || Data == null)
                throw new InvalidOperationException("Invalid SubnetElement");

            uint Response = 0;
            using (MemManager Mem = new MemManager())
            {
                DHCP_SUBNET_ELEMENT_DATA_V5 element = this.Data.ConvertToNative(Mem);

                Response = NativeMethods.DhcpAddSubnetElementV5(server.ToString(), this.SubnetAddress.GetUIntAddress(), ref element);
            }

            if (Response != 0)
                throw new DhcpException(Response);
        }

        /// <summary>
        /// Remove this element from server's subnet (defined by SubnetAddress)
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <param name="force">Force deletion of element's affected clients</param>
        /// <remarks>SubnetAddress and Data must be defined before calling Remove()</remarks>
        public void Remove(DhcpIpAddress server, bool force)
        {
            if (this.SubnetAddress == null || Data == null)
                throw new InvalidOperationException("Invalid SubnetElement");

            uint Response = 0;
            using (MemManager Mem = new MemManager())
            {
                DHCP_SUBNET_ELEMENT_DATA_V5 element = this.Data.ConvertToNative(Mem);

                Response = NativeMethods.DhcpRemoveSubnetElementV5(server.ToString(), this.SubnetAddress.GetUIntAddress(), ref element, (force ? DHCP_FORCE_FLAG.DhcpFullForce : DHCP_FORCE_FLAG.DhcpNoForce));
            }

            if (Response != 0)
                throw new DhcpException(Response);

        }

        /// <summary>
        /// Enumerate all elements based on Data's type
        /// </summary>
        /// <param name="server">Server IP</param>
        /// <returns>An array of subnet elements</returns>
        /// <remarks>SubnetAddress and Data must be defined before calling EnumAll()</remarks>
        public DhcpSubnetElementData[] EnumAll(DhcpIpAddress server)
        {
            if (this.SubnetAddress == null || Data == null)
                throw new InvalidOperationException("Invalid SubnetElement");

            DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5 NativeArray;
            List<DhcpSubnetElementData> elements = new List<DhcpSubnetElementData>();
            IntPtr retPtr, iPtr;
            UInt32 rHandle = 0, OptionsRead = 0, OptionsTotal = 0, Response = 0;

            for ( ; ; )
            {
                Response = NativeMethods.DhcpEnumSubnetElementsV5(server.ToString(), this.SubnetAddress.GetUIntAddress(), 
                    (DHCP_SUBNET_ELEMENT_TYPE_V5)this.Data.Type, ref rHandle, UInt32.MaxValue, out retPtr, out OptionsRead, out OptionsTotal);

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
                NativeArray = (DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5)Marshal.PtrToStructure<DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5>(retPtr);

                for (int i = 0; i < NativeArray.NumElements; ++i)
                {
                    iPtr = (IntPtr)(NativeArray.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_SUBNET_ELEMENT_DATA_V5>()));
                    DHCP_SUBNET_ELEMENT_DATA_V5 element = (DHCP_SUBNET_ELEMENT_DATA_V5)Marshal.PtrToStructure<DHCP_SUBNET_ELEMENT_DATA_V5>(iPtr);
                    elements.Add(DhcpSubnetElementData.CovertFromNative(element));
                }

                //free on last successful call
                if (Response == 0)
                    NativeMethods.DhcpRpcFreeMemory(retPtr);
            }
            return elements.ToArray();
        }
    }

    #region SubnetElementData...
    /// <summary>
    /// Base class that defines a <see cref="DhcpSubnetElement"/> data
    /// </summary>
    public abstract class DhcpSubnetElementData
    {
        /// <summary>
        /// Subnet element data type
        /// </summary>
        public DhcpSubnetElementDataType Type;
        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpSubnetElementData() { }
        /// <summary>
        /// Set base to type
        /// </summary>
        /// <param name="type">Type of subnet element data</param>
        public DhcpSubnetElementData(DhcpSubnetElementDataType type) { this.Type = type; }

        /// <summary>
        /// Derived classes must implement a conversion from their type to their native equivalent
        /// </summary>
        /// <param name="Mem">Tracks memory allocations</param>
        /// <returns>The native equivalent</returns>
        internal abstract DHCP_SUBNET_ELEMENT_DATA_V5 ConvertToNative(MemManager Mem);

        /// <summary>
        /// Converts the native subnet element type its SMO form based on type
        /// </summary>
        /// <param name="nativeElement">A native subnet element</param>
        /// <returns>The converted SMO</returns>
        internal static DhcpSubnetElementData CovertFromNative(DHCP_SUBNET_ELEMENT_DATA_V5 nativeElement)
        {
            switch (nativeElement.ElementType)
            {
                case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpReservedIps:
                    return new DhcpIpReservation(nativeElement);
                   
                case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRanges:
                case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpExcludedIpRanges:
                case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpOnly:
                    return new DhcpIpRange(nativeElement);
                    
                case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesBootpOnly:
                case DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpBootp:
                    return new BootpIpRange(nativeElement);
                   
                default:
                    throw new NotImplementedException("SubnetElementData Type: " + nativeElement.ElementType);
            }
        }

    }

    /// <summary>
    /// Defines a IP range used for a subnet element (feature)
    /// </summary>
    /// <remarks>This class supports types: <see cref="DhcpSubnetElementDataType.DhcpIpRanges"/>, <see cref="DhcpSubnetElementDataType.DhcpExcludedIpRanges"/>, or <see cref="DhcpSubnetElementDataType.DhcpIpRangesDhcpOnly"/></remarks>
    public class DhcpIpRange : DhcpSubnetElementData
    {
        /// <summary>
        /// Start IP of range
        /// </summary>
        public DhcpIpAddress StartAddress;
        /// <summary>
        /// End IP of range
        /// </summary>
        public DhcpIpAddress EndAddress;
        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpIpRange() { }
        /// <summary>
        /// Sets new DhcpIpRange to type
        /// </summary>
        /// <param name="type">Type of new DhcpIpRange</param>
        public DhcpIpRange(DhcpSubnetElementDataType type) : base(type) { }

        /// <summary>
        /// Internal constructor to create a SMO DhcpIpRange from its native form
        /// </summary>
        /// <param name="nativeElement">A native DHCP IP range subnet element</param>
        internal DhcpIpRange(DHCP_SUBNET_ELEMENT_DATA_V5 nativeElement)
        {
    
            this.Type = (DhcpSubnetElementDataType)nativeElement.ElementType;
        
            DHCP_IP_RANGE ipr = (DHCP_IP_RANGE)Marshal.PtrToStructure(nativeElement.Data, typeof(DHCP_IP_RANGE));
            this.StartAddress = new DhcpIpAddress(ipr.StartAddress);
            this.EndAddress = new DhcpIpAddress(ipr.EndAddress);
        }

        /// <summary>
        /// Converts this object to its native subnet element form
        /// </summary>
        /// <param name="Mem">Tracks memeroy allocations</param>
        /// <returns>A native DHCP IP range subnet element</returns>
        /// <remarks>Requires StartAddress, EndAddress and proper type to be set before calling</remarks>
        internal override DHCP_SUBNET_ELEMENT_DATA_V5 ConvertToNative(MemManager Mem)
        {
            if (this.StartAddress == null)
                throw new InvalidOperationException("DhcpIpRange.StartAddress is null");

            if (this.EndAddress == null)
                throw new InvalidOperationException("DhcpIpRange.EndAddress is null");

            DHCP_IP_RANGE dipr = new DHCP_IP_RANGE();
            dipr.StartAddress = this.StartAddress.GetUIntAddress();
            dipr.EndAddress = this.EndAddress.GetUIntAddress();

            DHCP_SUBNET_ELEMENT_DATA_V5 element = new DHCP_SUBNET_ELEMENT_DATA_V5();
            switch (this.Type)
            {
                case DhcpSubnetElementDataType.DhcpIpRanges:
                    element.ElementType = DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRanges;
                    break;
                case DhcpSubnetElementDataType.DhcpExcludedIpRanges:
                    element.ElementType = DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpExcludedIpRanges;
                    break;
                case DhcpSubnetElementDataType.DhcpIpRangesDhcpOnly:
                    element.ElementType = DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpOnly;
                    break;
                default:
                    throw new DhcpException(1629);
            }
            element.Data = Mem.AllocStruct(dipr);
            return element;

        }

    }

    /// <summary>
    /// Defines a BOOTP IP range, an extension of a DHCP IP range
    /// </summary>
    /// <remarks>This class supports types: <see cref="DhcpSubnetElementDataType.DhcpIpRangesBootpOnly"/>, <see cref="DhcpSubnetElementDataType.DhcpIpRangesDhcpBootp"/></remarks>
    public class BootpIpRange : DhcpIpRange
    {
        /// <summary>
        /// Specifies the number of BOOTP clients with addresses served from this range.
        /// </summary>
        public UInt32 BootpAllocated;
        /// <summary>
        /// Specifies the maximum number of BOOTP clients this range is allowed to serve.
        /// </summary>
        public UInt32 MaxBootpAllowed;
        /// <summary>
        /// Dummy constructor
        /// </summary>
        public BootpIpRange() { this.MaxBootpAllowed = UInt32.MaxValue; }
        /// <summary>
        /// Sets new BootpIpRange to type
        /// </summary>
        /// <param name="type">Type of new BootpIpRange</param>
        public BootpIpRange(DhcpSubnetElementDataType type) : base(type) { this.MaxBootpAllowed = UInt32.MaxValue; }


        /// <summary>
        /// Internal constructor to create a SMO BootpIpRange from its native form
        /// </summary>
        /// <param name="nativeElement">A native BOOTP IP range subnet element</param>
        internal BootpIpRange(DHCP_SUBNET_ELEMENT_DATA_V5 nativeElement)
        {
            this.Type = (DhcpSubnetElementDataType)nativeElement.ElementType;

            DHCP_BOOTP_IP_RANGE ipr = (DHCP_BOOTP_IP_RANGE)Marshal.PtrToStructure(nativeElement.Data, typeof(DHCP_BOOTP_IP_RANGE));
            this.StartAddress = new DhcpIpAddress(ipr.StartAddress);
            this.EndAddress = new DhcpIpAddress(ipr.EndAddress);
            this.BootpAllocated = ipr.BootpAllocated;
            this.MaxBootpAllowed = ipr.MaxBootpAllowed;
        }

        /// <summary>
        /// Converts this object to its native subnet element form
        /// </summary>
        /// <param name="Mem">Tracks memeroy allocations</param>
        /// <returns>A native BOOTP IP range subnet element</returns>
        /// <remarks>Requires StartAddress, EndAddress and proper type to be set before calling</remarks>
        internal override DHCP_SUBNET_ELEMENT_DATA_V5 ConvertToNative(MemManager Mem)
        {
            if (this.StartAddress == null)
                throw new InvalidOperationException("BootpIpRange.StartAddress is null");

            if (this.EndAddress == null)
                throw new InvalidOperationException("BootpIpRange.EndAddress is null");

            DHCP_BOOTP_IP_RANGE bipr = new DHCP_BOOTP_IP_RANGE();
            bipr.StartAddress = this.StartAddress.GetUIntAddress();
            bipr.EndAddress = this.EndAddress.GetUIntAddress();
            bipr.MaxBootpAllowed = this.MaxBootpAllowed;
            bipr.BootpAllocated = this.BootpAllocated;

            DHCP_SUBNET_ELEMENT_DATA_V5 element = new DHCP_SUBNET_ELEMENT_DATA_V5();
            switch (this.Type)
            {
                case DhcpSubnetElementDataType.DhcpIpRangesBootpOnly:
                    element.ElementType = DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesBootpOnly;
                    break;
                case DhcpSubnetElementDataType.DhcpIpRangesDhcpBootp:
                    element.ElementType = DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpIpRangesDhcpBootp;
                    break;
                default:
                    throw new DhcpException(1629);
            }
            element.Data = Mem.AllocStruct(bipr);
            return element;
        }
        
    }

    /// <summary>
    /// Defines a DHCP IP reservation subenet element
    /// </summary>
    /// <remarks>This class supports type: <see cref="DhcpSubnetElementDataType.DhcpReservedIps"/></remarks>
    public class DhcpIpReservation : DhcpSubnetElementData
    {
        /// <summary>
        /// Reservation IP address
        /// </summary>
        public DhcpIpAddress ReservedIp;
        /// <summary>
        /// Reservation MAC address
        /// </summary>
        public DhcpMacAddress ReservedMac;
        /// <summary>
        /// Reservation client type (DHCP, BOOTP, or Both)
        /// </summary>
        public DhcpSubnetClientType bAllowedClientTypes;
        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpIpReservation() { this.Type = DhcpSubnetElementDataType.DhcpReservedIps; }

        /// <summary>
        /// Internal constructor to create a SMO DhcpIpReservation from its native form
        /// </summary>
        /// <param name="nativeElement">A native DHCP IP Reservation subnet element</param>
        internal DhcpIpReservation(DHCP_SUBNET_ELEMENT_DATA_V5 nativeElement)
        {
            this.Type = (DhcpSubnetElementDataType)nativeElement.ElementType;

            DHCP_IP_RESERVATION_V4 rez = (DHCP_IP_RESERVATION_V4)Marshal.PtrToStructure(nativeElement.Data, typeof(DHCP_IP_RESERVATION_V4));
            this.ReservedIp = new DhcpIpAddress(rez.ReservedIpAddress);
            this.bAllowedClientTypes = (DhcpSubnetClientType)rez.bAllowedClientTypes;

            DHCP_CLIENT_UID mac = (DHCP_CLIENT_UID)Marshal.PtrToStructure(rez.ReservedForClient, typeof(DHCP_CLIENT_UID));
            if (mac.DataLength < 5)
                return;
            byte[] bytes = new byte[(int)mac.DataLength - 5];
            Marshal.Copy((IntPtr)(mac.Data.ToInt32() + 5), bytes, 0, (int)mac.DataLength - 5);
            this.ReservedMac = new DhcpMacAddress(bytes);
        }

        /// <summary>
        /// Converts this object to its native subnet element form
        /// </summary>
        /// <param name="Mem">Tracks memeroy allocations</param>
        /// <returns>A native DHCP IP Reservation subnet element</returns>
        /// <remarks>Requires ReservedIp, ReservedMac and proper type to be set before calling</remarks>
        internal override DHCP_SUBNET_ELEMENT_DATA_V5 ConvertToNative(MemManager Mem)
        {
            if (this.ReservedIp == null)
                throw new InvalidOperationException("IpReservation.ReservedIp is null");

            if (this.ReservedMac == null)
                throw new InvalidOperationException("IpReservation.ReservedMac is null");

            DHCP_IP_RESERVATION_V4 rip = new DHCP_IP_RESERVATION_V4();
            rip.ReservedIpAddress = this.ReservedIp.GetUIntAddress();
            rip.bAllowedClientTypes = (byte)this.bAllowedClientTypes;

            //extra work for mac address
            Byte[] mac = this.ReservedMac.GetByteArray();
            DHCP_CLIENT_UID uid = new DHCP_CLIENT_UID();
            uid.DataLength = (uint)mac.Length;
            uid.Data = Mem.AllocByteArray(this.ReservedMac.GetByteArray());
            rip.ReservedForClient = Mem.AllocStruct(uid);

            DHCP_SUBNET_ELEMENT_DATA_V5 element = new DHCP_SUBNET_ELEMENT_DATA_V5();
            switch (this.Type)
            {
                case DhcpSubnetElementDataType.DhcpReservedIps:
                    element.ElementType = DHCP_SUBNET_ELEMENT_TYPE_V5.DhcpReservedIps;
                    break;
                default:
                    throw new DhcpException(1629);
            }
            element.Data = Mem.AllocStruct(rip);
            return element;
        }
    }
    #endregion

    #region Enums...
    /// <summary>
    /// Types of <see cref="DhcpSubnetElementData"/>
    /// </summary>
    /// <remarks>Use type 'DhcpIpRangesDhcpBootp' when the default address pool type is unknown, for example on an EnumAll() call</remarks>
    public enum DhcpSubnetElementDataType
    {
        /// <summary>
        /// This type seems to not be used and replaced by 'DhcpIpRangeDhcpOnly'
        /// </summary>
        DhcpIpRanges = 0,
        //DhcpSecondaryHosts = 1,
        /// <summary>
        /// This type represents a reservation on a subnet
        /// </summary>
        DhcpReservedIps = 2,
        /// <summary>
        /// This type represents an exclusion address pool
        /// </summary>
        DhcpExcludedIpRanges = 3,
        //enum 4 is missing from documentation...
        /// <summary>
        /// This type represents the default range of a DHCP subnet of type DHCP only
        /// </summary>
        DhcpIpRangesDhcpOnly = 5,
        /// <summary>
        /// This type represents the default address pool of a DHCP subnet of both DHCP/BOOTP types.
        /// </summary>
        DhcpIpRangesDhcpBootp = 6,
        /// <summary>
        /// This type represents the default range of a DHCP subnet of type BOOTP only
        /// </summary>
        DhcpIpRangesBootpOnly = 7
    }

    /// <summary>
    /// Client Types
    /// </summary>
    public enum DhcpSubnetClientType : byte
    {
        /// <summary>
        /// DHCP client
        /// </summary>
        Dhcp = 1,
        /// <summary>
        /// BOOTP client
        /// </summary>
        Bootp = 2,
        /// <summary>
        /// DHCP and BOOTP client
        /// </summary>
        Both = 3
    }
    #endregion
}

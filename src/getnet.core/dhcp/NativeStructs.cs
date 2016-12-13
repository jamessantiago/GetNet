using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;


namespace Dwo.Interop
{
    #region DHCPArrays...

    /// <summary>
    /// An array of IP addresses
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363353(VS.85).aspx">DHCP_IP_ARRAY [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_ARRAY
    {
        /// <summary>
        /// Number of IP addresses in Elements
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="System.UInt32">UInt32</see> values
        /// </summary>
        public IntPtr Elements;
    }

    /// <summary>
    /// An array of subnet data elements
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa813380(VS.85).aspx">DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5 [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5
    {
        /// <summary>
        /// Number of DHCP_SUBNET_ELEMENT_DATA_V5 in Elements
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pinter to an array of <see cref="DHCP_SUBNET_ELEMENT_DATA_V5">DHCP_SUBNET_ELEMENT_DATA_V5</see> values
        /// </summary>
        public IntPtr Elements;
    }

    /// <summary>
    /// An array of DHCP server option elements
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363357(VS.85).aspx">DHCP_OPTION_ARRAY [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_ARRAY
    {
        /// <summary>
        /// Number of DHCP_OPTION in Elements
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="DHCP_OPTION">DHCP_OPTION</see> values
        /// </summary>
        public IntPtr Elements;
    }

    /// <summary>
    /// An array of data elements associated with a DHCP option
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363358(VS.85).aspx">DHCP_OPTION_DATA [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_DATA
    {
        /// <summary>
        /// Number of DHCP_OPTION_DATA_ELEMENT in Elements
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="DHCP_OPTION_DATA_ELEMENT">DHCP_OPTION_DATA_ELEMENT</see> values
        /// </summary>
        public IntPtr Elements;
    }

    /// <summary>
    /// An array of client info structures
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363349(VS.85).aspx">DHCP_CLIENT_INFO_ARRAY [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_INFO_ARRAY
    {
        /// <summary>
        /// Number of DHCP_CLIENT_INFO in Elements
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Pointer to an array of <see cref="DHCP_CLIENT_INFO">DHCP_CLIENT_INFO</see> values
        /// </summary>
        public IntPtr Elements;
    }

    /// <summary>
    /// An array of DHCP option values
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/bb891963(VS.85).aspx">DHCP_OPTION_VALUE_ARRAY [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE_ARRAY
    {
        /// <summary>
        /// Number of DHCP_OPTION_VALUE in Elements
        /// </summary>
        public UInt32 NumElements;
        /// <summary>
        /// Point to an array of <see cref="DHCP_OPTION_VALUE">DHCP_OPTION_VALUE</see> values
        /// </summary>
        public IntPtr Elements;
    }
    #endregion

    #region DHCPEnums...

    /// <summary>
    /// Defines the set of possible attributes used to search DHCP client information records
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363371(VS.85).aspx">DHCP_SEARCH_INFO_TYPE [MSDN]</a></p>
    /// </remarks>
    internal enum DHCP_SEARCH_INFO_TYPE
    {
        /// <summary>
        /// Search by client IP
        /// </summary>
        DhcpClientIpAddress,
        /// <summary>
        /// Search by Mac
        /// </summary>
        DhcpClientHardwareAddress,
        /// <summary>
        /// Search by hostname
        /// </summary>
        DhcpClientName
    }

    /// <summary>
    /// Defines if an option's data is in array form or single value
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363365(VS.85).aspx">DHCP_OPTION_TYPE [MSDN]</a></p>
    /// </remarks>
    internal enum DHCP_OPTION_TYPE
    {
        /// <summary>
        /// Single value type
        /// </summary>
        DhcpUnaryElementTypeOption,
        /// <summary>
        /// Array value type
        /// </summary>
        DhcpArrayTypeOption
    }

    /// <summary>
    /// Defines the set of possible states for a subnet
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363387(VS.85).aspx">DHCP_SUBNET_STATE [MSDN]</a></p>
    /// </remarks>
    internal enum DHCP_SUBNET_STATE
    {
        /// <summary>
        /// Subnet Enabled
        /// </summary>
        DhcpSubnetEnabled,
        /// <summary>
        /// Subnet Disabled
        /// </summary>
        DhcpSubnetDisabled,
        /// <summary>
        /// Subnet Enabled and DHCP server is gateway
        /// </summary>
        DhcpSubnetEnabledSwitched,
        /// <summary>
        /// SUbnet Disabled and DHCP server is gateway
        /// </summary>
        DhcpSubnetDisabledSwitched,
        /// <summary>
        /// Subnet in invalid state
        /// </summary>
        DhcpSubnetInvalidState
    }

    
    /// <summary>
    /// Defines the set of possible subnet element types
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363385(VS.85).aspx">DHCP_SUBNET_ELEMENT_TYPE [MSDN]</a></p>
    /// <p><b>Notice</b> enum value 4 is missing.  I came to this because EnumSubnetElements was returning invalid parameter when<br/>
    /// the this enum was of normal numbering 0...6</p>
    /// </remarks>
    internal enum DHCP_SUBNET_ELEMENT_TYPE_V5
    {
        /// <summary>
        /// 0 - Use DhcpIpRangesDhcpOnly instead
        /// </summary>
        DhcpIpRanges = 0,
        /// <summary>
        /// 1 - Not Implemented
        /// </summary>
        DhcpSecondaryHosts = 1,
        /// <summary>
        /// 2 - IP reserverations
        /// </summary>
        DhcpReservedIps = 2,
        /// <summary>
        /// 3 - Excluded IP ranges or 'Pools'
        /// </summary>
        DhcpExcludedIpRanges = 3,
        //value 4 is missing from documentation...
        //I figured this out because enumsubnetelements for enums below was cause invalid parameter errors
        /// <summary>
        /// 5 - DHCP IP range only
        /// </summary>
        DhcpIpRangesDhcpOnly = 5,
        /// <summary>
        /// 6 - DHCP and/or Bootp ranges.  Use this type to get the default range for a subnet
        /// </summary>
        DhcpIpRangesDhcpBootp = 6,
        /// <summary>
        /// 7 - BOOTP IP range only
        /// </summary>
        DhcpIpRangesBootpOnly = 7
    }

    /// <summary>
    /// Defines the set of flags describing the force level of a DHCP subnet element deletion operation
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363350(VS.85).aspx">DHCP_FORCE_FLAG [MSDN]</a></p>
    /// </remarks>
    internal enum DHCP_FORCE_FLAG
    {
        /// <summary>
        /// Delete subnet element and all affected records
        /// </summary>
        DhcpFullForce,
        /// <summary>
        /// Delete only subnet element
        /// </summary>
        DhcpNoForce
    }

    /// <summary>
    /// Defines the set of possible DHCP option scopes
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363363(VS.85).aspx">DHCP_OPTION_SCOPE_TYPE [MSDN]</a></p>
    /// </remarks>
    internal enum DHCP_OPTION_SCOPE_TYPE
    {
        /// <summary>
        /// Not Implemented
        /// </summary>
        DhcpDefaultOptions,
        /// <summary>
        /// Server Options
        /// </summary>
        DhcpGlobalOptions,
        /// <summary>
        /// Subnet Options
        /// </summary>
        DhcpSubnetOptions,
        /// <summary>
        /// Reservation Options
        /// </summary>
        DhcpReservedOptions,
        /// <summary>
        /// Not Implemented
        /// </summary>
        DhcpMScopeOptions
    }

    /// <summary>
    /// Defines the set of formats that represent DHCP option data
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363360(VS.85).aspx">DHCP_OPTION_DATA_TYPE [MSDN]</a></p>
    /// </remarks>
    internal enum DHCP_OPTION_DATA_TYPE
    {
        /// <summary>
        /// Data type of byte
        /// </summary>
        DhcpByteOption,
        /// <summary>
        /// Data type of WORD (uint16)
        /// </summary>
        DhcpWordOption,
        /// <summary>
        /// Data type of DWORD (uint32)
        /// </summary>
        DhcpDWordOption,
        /// <summary>
        /// Data type of DWORDDWORD (uint64)
        /// </summary>
        DhcpDWordDWordOption,
        /// <summary>
        /// Data type of IP address (uint)
        /// </summary>
        DhcpIpAddressOption,
        /// <summary>
        /// Data type of string
        /// </summary>
        DhcpStringDataOption,
        /// <summary>
        /// Not Implemented
        /// </summary>
        DhcpBinaryDataOption,
        /// <summary>
        /// Not Implemented
        /// </summary>
        DhcpEncapsulatedDataOption,
        /// <summary>
        /// Not Implemented
        /// </summary>
        DhcpIpv6AddressOption
    }

    #endregion

    #region DhcpOptionDataElement...

    /// <summary>
    /// Defines a data element present (either singly or as a member of an array) within a DHCP_OPTION_DATA structure<br/>
    /// This structure mimics a C/C++ Union
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363359(VS.85).aspx">DHCP_OPTION_DATA_ELEMENT [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>
    /// [StructLayout(LayoutKind.Explicit, Size = 12)]
    /// internal struct DHCP_OPTION_DATA_ELEMENT
    /// {
    ///     [FieldOffset(0)]
    ///     public DHCP_OPTION_DATA_TYPE OptionType;
    ///     [FieldOffset(4)]
    ///     public byte ByteOption;
    ///     [FieldOffset(4)]
    ///     public UInt16 WordOption;
    ///     [FieldOffset(4)]
    ///     public UInt32 DWordOption;
    ///     [FieldOffset(4)]
    ///     public DWORD_DWORD DWordDWordOption;
    ///     [FieldOffset(4)]
    ///     public IntPtr StringOption;
    ///     [FieldOffset(4)]
    ///     public DHCP_BINARY_DATA BinDataOption;
    /// }
    /// </code>
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    internal struct DHCP_OPTION_DATA_ELEMENT
    {
        /// <summary>
        /// Type of option data
        /// </summary>
        [FieldOffset(0)]
        public DHCP_OPTION_DATA_TYPE OptionType;
        /// <summary>
        /// Union member byte
        /// </summary>
        [FieldOffset(4)]
        public byte ByteOption;
        /// <summary>
        /// Union member UInt16
        /// </summary>
        [FieldOffset(4)]
        public UInt16 WordOption;
        /// <summary>
        /// Union member UInt32
        /// </summary>
        [FieldOffset(4)]
        public UInt32 DWordOption;
        /// <summary>
        /// Union member DWORD_DWORD (64bit)
        /// </summary>
        [FieldOffset(4)]
        public DWORD_DWORD DWordDWordOption;
        /// <summary>
        /// Union member String
        /// </summary>
        [FieldOffset(4)]
        public IntPtr StringOption;
        /// <summary>
        /// Union member Byte Array
        /// </summary>
        [FieldOffset(4)]
        public DHCP_BINARY_DATA BinDataOption;
    }
    #endregion

    #region DhcpOptionScopeInfo...

    /// <summary>
    /// Defines information about the options provided for a certain DHCP scope<br/>
    /// This structure mimics a C/C++ Union
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363361(VS.85).aspx">DHCP_OPTION_SCOPE_INFO [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>
    /// [StructLayout(LayoutKind.Explicit, Size = 12)]
    /// internal struct DHCP_OPTION_SCOPE_INFO
    /// {
    ///     [FieldOffset(0)]
    ///     public DHCP_OPTION_SCOPE_TYPE ScopeType;
    ///     [FieldOffset(4)]
    ///     public UInt32 SubnetScopeInfo;
    ///     [FieldOffset(4)]
    ///     public DHCP_RESERVED_SCOPE ReservedScopeInfo;
    ///     [FieldOffset(4)]
    ///     public IntPtr GlobalScopeInfo;
    /// }
    /// </code>
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    internal struct DHCP_OPTION_SCOPE_INFO
    {
        /// <summary>
        /// Scope Type (Server, Subnet, or Reservation)
        /// </summary>
        [FieldOffset(0)]
        public DHCP_OPTION_SCOPE_TYPE ScopeType;
        /// <summary>
        /// Union member Subnet network IP
        /// </summary>
        [FieldOffset(4)]
        public UInt32 SubnetScopeInfo;
        /// <summary>
        /// Union member for reservation (subnet and reseration IP)
        /// </summary>
        [FieldOffset(4)]
        public DHCP_RESERVED_SCOPE ReservedScopeInfo;
        /// <summary>
        /// Union member Server (use IntPtr.Zero)
        /// </summary>
        [FieldOffset(4)]
        public IntPtr GlobalScopeInfo;
    }
    #endregion

    #region DhcpSearchInfo...
    /// <summary>
    /// Defines the DHCP client record data used to search against for particular server operations<br/>
    /// This structure mimics a C/C++ Union
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363370(VS.85).aspx">DHCP_SEARCH_INFO [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>
    /// [StructLayout(LayoutKind.Explicit, Size = 12)]
    /// internal struct DHCP_SEARCH_INFO
    /// {
    ///     [FieldOffset(0)]
    ///     public DHCP_SEARCH_INFO_TYPE SearchType;
    ///     [FieldOffset(4)]
    ///     public DHCP_CLIENT_UID ClientHardwareAddress;
    ///     [FieldOffset(4)]
    ///     public UInt32 ClientIpAddress;
    ///     [FieldOffset(4)]
    ///     public IntPtr ClientName;
    /// }
    /// </code>
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    internal struct DHCP_SEARCH_INFO
    {
        /// <summary>
        /// Search type (IP, MAC, or Hostname)
        /// </summary>
        [FieldOffset(0)]
        public DHCP_SEARCH_INFO_TYPE SearchType;
        /// <summary>
        /// Union member for MAC type
        /// </summary>
        [FieldOffset(4)]
        public DHCP_CLIENT_UID ClientHardwareAddress;
        /// <summary>
        /// Union member for IP type
        /// </summary>
        [FieldOffset(4)]
        public UInt32 ClientIpAddress;
        /// <summary>
        /// Union member for Hostname type
        /// </summary>
        [FieldOffset(4)]
        public IntPtr ClientName;
    }
    #endregion

    #region DhcpStructs...

    /// <summary>
    /// Defines information describing a subnet
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363386(VS.85).aspx">DHCP_SUBNET_INFO [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_SUBNET_INFO
    {
        /// <summary>
        /// Subnet network IP
        /// </summary>
        public UInt32 SubnetAddress;
        /// <summary>
        /// Subnet network mask
        /// </summary>
        public UInt32 SubnetMask;
        /// <summary>
        /// Name value for subnet
        /// </summary>
        public String SubnetName;
        /// <summary>
        /// Description value for subnet
        /// </summary>
        public String SubnetComment;
        /// <summary>
        /// Ignored
        /// </summary>
        public DHCP_HOST_INFO PrimaryHost;
        /// <summary>
        /// Subnet State (Enable, Disabled...)
        /// </summary>
        public DHCP_SUBNET_STATE SubnetState;
    }

    /// <summary>
    /// Defines a client IP reservation
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363355(VS.85).aspx">DHCP_IP_RESERVATION_V4 [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_RESERVATION_V4
    {
        /// <summary>
        /// IP address of reservation
        /// </summary>
        public UInt32 ReservedIpAddress;
        /// <summary>
        /// Pointer to <see cref="DHCP_CLIENT_UID"/> (MAC address)
        /// </summary>
        public IntPtr ReservedForClient; //DHCP_CLIENT_UID*
        /// <summary>
        /// Allow client type (DHCP, BOOTP, or Both)
        /// </summary>
        public byte bAllowedClientTypes;
    }

    /// <summary>
    /// Defines a suite of IPs for lease to BOOTP-specific clients
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363342(VS.85).aspx">DHCP_BOOTP_IP_RANGE [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_BOOTP_IP_RANGE
    {
        /// <summary>
        /// Start IP address
        /// </summary>
        public UInt32 StartAddress;
        /// <summary>
        /// End IP address
        /// </summary>
        public UInt32 EndAddress;
        /// <summary>
        /// Number of allocation BOOTP clients
        /// </summary>
        public UInt32 BootpAllocated;
        /// <summary>
        /// Max allowed BOOTP clients
        /// </summary>
        public UInt32 MaxBootpAllowed;
        //A ULONG is a unsigned long in win32, a long is 32bits.  A ulong in c# is 64bits
    }

    /// <summary>
    /// Defines a range of IP addresses
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363354(VS.85).aspx">DHCP_IP_RANGE [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_IP_RANGE
    {
        /// <summary>
        /// Start IP address
        /// </summary>
        public UInt32 StartAddress;
        /// <summary>
        /// End IP address
        /// </summary>
        public UInt32 EndAddress;
    }

    /// <summary>
    /// Defines an element that describes a feature or restriction of a subnet
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363384(VS.85).aspx">DHCP_SUBNET_ELEMENT_DATA_V5 [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_SUBNET_ELEMENT_DATA_V5
    {
        /// <summary>
        /// Type of subnet element data
        /// </summary>
        public DHCP_SUBNET_ELEMENT_TYPE_V5 ElementType;
        /// <summary>
        /// Pointer to a DHCP structure based on ElementType.<br/>
        /// Possible type: DHCP_IP_RANGE, DHCP_BOOTP_IP_RANGE, DHCP_IP_RESERVATION_V4, and DHCP_HOST_INFO 
        /// </summary>
        public IntPtr Data;
    }

    /// <summary>
    /// Defines a reserved DHCP scope
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363369(VS.85).aspx">DHCP_RESERVED_SCOPE [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_RESERVED_SCOPE
    {
        /// <summary>
        /// IP address of reservation
        /// </summary>
        public UInt32 ReservedIpAddress;
        /// <summary>
        /// IP address of subnet that owner reservation
        /// </summary>
        public UInt32 ReservedIpSubnetAddress;
    }

    /// <summary>
    /// Defines a single DHCP option and any default data elements associated with it
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363356(VS.85).aspx">DHCP_OPTION [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_OPTION
    {
        /// <summary>
        /// DHCP Option ID (<a href="http://tools.ietf.org/html/rfc2132">rfc2132</a>)
        /// </summary>
        public UInt32 OptionID;
        /// <summary>
        /// Option name value
        /// </summary>
        public string OptionName;
        /// <summary>
        /// Option desciption value
        /// </summary>
        public string OptionComment;
        /// <summary>
        /// Default value of this option
        /// </summary>
        public DHCP_OPTION_DATA DefaultValue;
        /// <summary>
        /// Data type of this option
        /// </summary>
        public DHCP_OPTION_TYPE OptionType;
    }

    /// <summary>
    /// Defines a DHCP option value (just the option data with an associated ID tag)
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363367(VS.85).aspx">DHCP_OPTION_VALUE [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_OPTION_VALUE
    {
        /// <summary>
        /// DHCP Option ID (<a href="http://tools.ietf.org/html/rfc2132">rfc2132</a>)
        /// </summary>
        public UInt32 OptionID;
        /// <summary>
        /// Option data value(s)
        /// </summary>
        public DHCP_OPTION_DATA Value;
    }

    /// <summary>
    /// Defines information on a DHCP server (host)
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363352(VS.85).aspx">DHCP_HOST_INFO [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_HOST_INFO
    {
        /// <summary>
        /// IP address of host
        /// </summary>
        public UInt32 IpAddress;
        /// <summary>
        /// Host's netbios name
        /// </summary>
        public string NetBiosName;
        /// <summary>
        /// Hostname
        /// </summary>
        public string HostName;
    }

    /// <summary>
    /// Defines a client information record used by the DHCP server
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363348(VS.85).aspx">DHCP_CLIENT_INFO [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DHCP_CLIENT_INFO
    {
        /// <summary>
        /// IP address of client lease
        /// </summary>
        internal UInt32 ClientIpAddress;
        /// <summary>
        /// Subnet mask of client lease
        /// </summary>
        public UInt32 SubnetMask;
        /// <summary>
        /// MAC address of client lease
        /// </summary>
        public DHCP_CLIENT_UID ClientHardwareAddress;
        /// <summary>
        /// Client hostname
        /// </summary>
        public string ClientName;
        /// <summary>
        /// Client description value
        /// </summary>
        public string ClientComment;
        /// <summary>
        /// FILETIME when lease expires
        /// </summary>
        public DATE_TIME ClientLeaseExpires;
        /// <summary>
        /// Ignored
        /// </summary>
        public DHCP_HOST_INFO OwnerHost;
    }

    /// <summary>
    /// Defines a 64-bit integer value
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363388(VS.85).aspx">DWORD_DWORD [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DWORD_DWORD
    {
        /// <summary>
        /// Upper 32bits
        /// </summary>
        public UInt32 UpperWord1;
        /// <summary>
        /// Lower 32bits
        /// </summary>
        public UInt32 LowerWord2;
    }

    /// <summary>
    /// Defines a Byte array representing a hardware/mac address
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363329(VS.85).aspx">DHCP_BINARY_DATA [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_CLIENT_UID
    {
        /// <summary>
        /// Size of Byte array
        /// </summary>
        public UInt32 DataLength;
        /// <summary>
        /// Pointer to Byte array
        /// </summary>
        public IntPtr Data;
    }

    /// <summary>
    /// Defines an opaque blob of binary data
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363329(VS.85).aspx">DHCP_BINARY_DATA [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DHCP_BINARY_DATA
    {
        /// <summary>
        /// Size of Byte array
        /// </summary>
        public UInt32 DataLength;
        /// <summary>
        /// Pointer to Byte array
        /// </summary>
        public IntPtr Data;
    }

    /// <summary>
    /// Defines a 64-bit integer value that contains a date/time, expressed in FILETIME format
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363267(VS.85).aspx">DATE_TIME [MSDN]</a></p>
    /// <h5>Interop Attributes</h5>
    /// <code>[StructLayout(LayoutKind.Sequential)]</code>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DATE_TIME
    {
        /// <summary>
        /// Lower 32bits of FILETIME
        /// </summary>
        public UInt32 dwLowDateTime;
        /// <summary>
        /// Upper 32bits of FILETIME
        /// </summary>
        public UInt32 dwHighDateTime;

        /// <summary>
        /// Constructor takes a .Net DateTime and creates a DHCPSAPI DATE_TIME
        /// </summary>
        /// <param name="date">.Net DateTime type to convert</param>
        public DATE_TIME(DateTime date)
        {
            if (date == DateTime.MaxValue)
            {
                this.dwLowDateTime = this.dwHighDateTime = UInt32.MaxValue;
                return;
            }
            if (date == DateTime.MinValue)
            {
                this.dwLowDateTime = this.dwHighDateTime = UInt32.MinValue;
                return;
            }

            this.dwLowDateTime = (UInt32)(date.ToFileTime() & 0xFFFFFFFF);
            this.dwHighDateTime = (UInt32)((date.ToFileTime() & 0x7FFFFFFF00000000) >> 32);
        }
        
        /// <summary>
        /// Convert this object to a .Net DateTime
        /// </summary>
        public DateTime Convert()
        {
            
            if (this.dwHighDateTime == 0 && this.dwLowDateTime == 0)
                return DateTime.MinValue;

            if (this.dwHighDateTime == int.MaxValue && this.dwLowDateTime == UInt32.MaxValue)
                return DateTime.MaxValue;
            
            return DateTime.FromFileTime((((long)this.dwHighDateTime) << 32) | (UInt32)this.dwLowDateTime);
        }

        /// <summary>
        /// String value of .Net DateTime of this object
        /// </summary>
        /// <returns>String value of .Net DateTime</returns>
        public override string ToString()
        {
            return this.Convert().ToString();
        }
    }

	[StructLayout(LayoutKind.Sequential)]
	internal struct SCOPE_MIB_INFO_V5
	{
		public UInt32 Subnet;
		public UInt32 NumAddressesInUse;
		public UInt32 NumAddressesFree;
		public UInt32 NumPendingOffers;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct DHCP_MIB_INFO_V5
	{
		public UInt32 Discovers;
		public UInt32 Offers;
		public UInt32 Requests;
		public UInt32 Acks;
		public UInt32 Naks;
		public UInt32 Declines;
		public UInt32 Releases;
		public DATE_TIME ServerStartTime;
		public UInt32 QtnPctQtnLeases;
		public UInt32 QtnProbationLeases;
		public UInt32 QtnNonQtnLeases;
		public UInt32 QtnExemptLeases;
		public UInt32 QtnCapableClients;
		public UInt32 QtnIASErrors;
		public UInt32 DelayedOffers;
		public UInt32 ScopesWithDelayedOffers;
		public UInt32 Scopes;
		public SCOPE_MIB_INFO_V5 ScopeInfo;
	}

    #endregion


    #region DhcpsapiError

    /// <summary>
    /// DHCP Server Management API Error Codes
    /// </summary>
    /// <remarks>
    /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363378(VS.85).aspx">DHCP Server Management API Error Codes [MSDN]</a></p>
    /// </remarks>
    internal enum DhcpsapiErrorType : uint
    {
        [Description("The DHCP server registry initialization parameters are incorrect.")]
        ERROR_DHCP_REGISTRY_INIT_FAILED = 20000,
        [Description("The DHCP server was unable to open the database of DHCP clients.")]
        ERROR_DHCP_DATABASE_INIT_FAILED = 20001,
        [Description("The DHCP server was unable to start as a Remote Procedure Call (RPC) server.")]
        ERROR_DHCP_RPC_INIT_FAILED = 20002,
        [Description("The DHCP server was unable to establish a socket connection.")]
        ERROR_DHCP_NETWORK_INIT_FAILED = 20003,
        [Description("The specified subnet already exists on the DHCP server.")]
        ERROR_DHCP_SUBNET_EXISTS = 20004,
        [Description("The specified subnet does not exist on the DHCP server.")]
        ERROR_DHCP_SUBNET_NOT_PRESENT = 20005,
        [Description("The primary host information for the specified subnet was not found on the DHCP server.")]
        ERROR_DHCP_PRIMARY_NOT_FOUND = 20006,
        [Description("The specified DHCP element has been used by a client and cannot be removed.")]
        ERROR_DHCP_ELEMENT_CANT_REMOVE = 20007,
        [Description("The specified option already exists on the DHCP server.")]
        ERROR_DHCP_OPTION_EXISTS = 20009,
        [Description("The specified option does not exist on the DHCP server.")]
        ERROR_DHCP_OPTION_NOT_PRESENT = 20010,
        [Description("The specified IP address is not available.")]
        ERROR_DHCP_ADDRESS_NOT_AVAILABLE = 20011,
        [Description("The specified IP address range has all of its member addresses leased.")]
        ERROR_DHCP_RANGE_FULL = 20012,
        [Description("An error occurred while accessing the DHCP JET database. For more information about this error, please look at the DHCP server event log.")]
        ERROR_DHCP_JET_ERROR = 20013,
        [Description("The specified client already exists in the database.")]
        ERROR_DHCP_CLIENT_EXISTS = 20014,
        [Description("The DHCP server received an invalid message.")]
        ERROR_DHCP_INVALID_DHCP_MESSAGE = 20015,
        [Description("The DHCP server received an invalid message from the client.")]
        ERROR_DHCP_INVALID_DHCP_CLIENT = 20016,
        [Description("The DHCP server is currently paused.")]
        ERROR_DHCP_SERVICE_PAUSED = 20017,
        [Description("The specified DHCP client is not a reserved client.")]
        ERROR_DHCP_NOT_RESERVED_CLIENT = 20018,
        [Description("The specified DHCP client is a reserved client.")]
        ERROR_DHCP_RESERVED_CLIENT = 20019,
        [Description("The specified IP address range is too small.")]
        ERROR_DHCP_RANGE_TOO_SMALL = 20020,
        [Description("The specified IP address range is already defined on the DHCP server.")]
        ERROR_DHCP_IPRANGE_EXISTS = 20021,
        [Description("The specified Reservation is currently taken by another client.")]
        ERROR_DHCP_RESERVEDIP_EXISTS = 20022,
        [Description("The specified IP address range either overlaps with an existing range or is invalid.")]
        ERROR_DHCP_INVALID_RANGE = 20023,
        [Description("The specified IP address range is an extension of an existing range.")]
        ERROR_DHCP_RANGE_EXTENDED = 20024,
        [Description("The specified IP address range extension is too small. The number of addresses in the extension must be a multiple of 32.")]
        ERROR_DHCP_RANGE_EXTENSION_TOO_SMALL = 20025,
        [Description("An attempt was made to extend the IP address range to a value less than the specified backward extension. The number of addresses in the extension must be a multiple of 32.")]
        ERROR_DHCP_WARNING_RANGE_EXTENDED_LESS = 20026,
        [Description("The DHCP database needs to be upgraded to a newer format. For more information, refer to the DHCP server event log.")]
        ERROR_DHCP_JET_CONV_REQUIRED = 20027,
        [Description("The format of the bootstrap protocol file table is incorrect.")]
        ERROR_DHCP_SERVER_INVALID_BOOT_FILE_TABLE = 20028,
        [Description("A boot file name specified in the bootstrap protocol file table is unrecognized or invalid.")]
        ERROR_DHCP_SERVER_UNKNOWN_BOOT_FILE_NAME = 20029,
        [Description("The specified superscope name is too long.")]
        ERROR_DHCP_SUPER_SCOPE_NAME_TOO_LONG = 20030,
        [Description("The specified IP address is already in use.")]
        ERROR_DHCP_IP_ADDRESS_IN_USE = 20032,
        [Description("The specified path to the DHCP audit log file is too long.")]
        ERROR_DHCP_LOG_FILE_PATH_TOO_LONG = 20033,
        [Description("The DHCP server received a request for a valid IP address not administered by the server.")]
        ERROR_DHCP_UNSUPPORTED_CLIENT = 20034,
        [Description("The DHCP server failed to receive a notification when the interface list changed, therefore some of the interfaces will not be enabled on the server.")]
        ERROR_DHCP_SERVER_INTERFACE_NOTIFICATION_EVENT = 20035,
        [Description("The DHCP database needs to be upgraded to a newer format (JET97). For more information, refer to the DHCP server event log.")]
        ERROR_DHCP_JET97_CONV_REQUIRED = 20036,
        [Description("The DHCP server cannot determine if it has the authority to run, and is not servicing clients on the network. This rogue status may be due to network problems or insufficient server resources.")]
        ERROR_DHCP_ROGUE_INIT_FAILED = 20037,
        [Description("The DHCP service is shutting down because another DHCP server is active on the network.")]
        ERROR_DHCP_ROGUE_SAMSHUTDOWN = 20038,
        [Description("The DHCP server does not have the authority to run, and is not servicing clients on the network.")]
        ERROR_DHCP_ROGUE_NOT_AUTHORIZED = 20039,
        [Description("The DHCP server is unable to contact the directory service for this domain. The DHCP server will continue to attempt to contact the directory service. During this time, no clients on the network will be serviced.")]
        ERROR_DHCP_ROGUE_DS_UNREACHABLE = 20040,
        [Description("The DHCP server's authorization information conflicts with that of another DHCP server on the network.")]
        ERROR_DHCP_ROGUE_DS_CONFLICT = 20041,
        [Description("The DHCP server is ignoring a request from another DHCP server because the second server is a member of a different directory service enterprise.")]
        ERROR_DHCP_ROGUE_NOT_OUR_ENTERPRISE = 20042,
        [Description("The DHCP server has detected a directory service environment on the network. If there is a directory service on the network, the DHCP server can only run if it is a part of the directory service. Since the server ostensibly belongs to a workgroup, it is terminating.")]
        ERROR_DHCP_STANDALONE_IN_DS = 20043,
        [Description("The specified DHCP class name is unknown or invalid.")]
        ERROR_DHCP_CLASS_NOT_FOUND = 20044,
        [Description("The specified DHCP class name (or information) is already in use.")]
        ERROR_DHCP_CLASS_ALREADY_EXISTS = 20045,
        [Description("The specified DHCP scope name is too long; the scope name must not exceed 256 characters.")]
        ERROR_DHCP_SCOPE_NAME_TOO_LONG = 20046,
        [Description("The default scope is already configured on the server.")]
        ERROR_DHCP_DEFAULT_SCOPE_EXISTS = 20047,
        [Description("The Dynamic BOOTP attribute cannot be turned on or off.")]
        ERROR_DHCP_CANT_CHANGE_ATTRIBUTE = 20048,
        [Description("Conversion of a scope to a \"DHCP Only\" scope or to a \"BOOTP Only\" scope is not allowed when the scope contains other DHCP and BOOTP clients. Either the DHCP or BOOTP clients should be specifically deleted before converting the scope to the other type.")]
        ERROR_DHCP_IPRANGE_CONV_ILLEGAL = 20049,
        [Description("The network has changed. Retry this operation after checking for network changes. Network changes may be caused by interfaces that are new or invalid, or by IP addresses that are new or invalid.")]
        ERROR_DHCP_NETWORK_CHANGED = 20050,
        [Description("The bindings to internal IP addresses cannot be modified.")]
        ERROR_DHCP_CANNOT_MODIFY_BINDINGS = 20051,
        [Description("The DHCP scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
        ERROR_DHCP_SUBNET_EXISTS_2 = 20052,
        [Description("The DHCP multicast scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
        ERROR_DHCP_MSCOPE_EXISTS = 20053,
        [Description("The multicast scope range must have at least 256 IP addresses.")]
        ERROR_DHCP_MSCOPE_RANGE_TOO_SMALL = 20054,
        [Description("The DHCP server could not contact Active Directory.")]
        ERROR_DDS_NO_DS_AVAILABLE = 20070,
        [Description("The DHCP service root could not be found in Active Directory.")]
        ERROR_DDS_NO_DHCP_ROOT = 20071,
        [Description("An unexpected error occurred while accessing Active Directory.")]
        ERROR_DDS_UNEXPECTED_ERROR = 20072,
        [Description("There were too many errors to proceed.")]
        ERROR_DDS_TOO_MANY_ERRORS = 20073,
        [Description("A DHCP service could not be found.")]
        ERROR_DDS_DHCP_SERVER_NOT_FOUND = 20074,
        [Description("The specified DHCP options are already present in Active Directory.")]
        ERROR_DDS_OPTION_ALREADY_EXISTS = 20075,
        [Description("The specified DHCP options are not present in Active Directory.")]
        ERROR_DDS_OPTION_DOES_NOT_EXIST = 20076,
        [Description("The specified DHCP classes are already present in Active Directory.")]
        ERROR_DDS_CLASS_EXISTS = 20077,
        [Description("The specified DHCP classes are not present in Active Directory.")]
        ERROR_DDS_CLASS_DOES_NOT_EXIST = 20078,
        [Description("The specified DHCP servers are already present in Active Directory.")]
        ERROR_DDS_SERVER_ALREADY_EXISTS = 20079,
        [Description("The specified DHCP servers are not present in Active Directory.")]
        ERROR_DDS_SERVER_DOES_NOT_EXIST = 20080,
        [Description("The specified DHCP server address does not correspond to the identified DHCP server name.")]
        ERROR_DDS_SERVER_ADDRESS_MISMATCH = 20081,
        [Description("The specified subnets are already present in Active Directory.")]
        ERROR_DDS_SUBNET_EXISTS = 20082,
        [Description("The specified subnet belongs to a different superscope.")]
        ERROR_DDS_SUBNET_HAS_DIFF_SUPER_SCOPE = 20083,
        [Description("The specified subnet is not present in Active Directory.")]
        ERROR_DDS_SUBNET_NOT_PRESENT = 20084,
        [Description("The specified reservation is not present in Active Directory.")]
        ERROR_DDS_RESERVATION_NOT_PRESENT = 20085,
        [Description("The specified reservation conflicts with another reservation present in Active Directory.")]
        ERROR_DDS_RESERVATION_CONFLICT = 20086,
        [Description("The specified IP address range conflicts with another IP range present in Active Directory.")]
        ERROR_DDS_POSSIBLE_RANGE_CONFLICT = 20087,
        [Description("The specified IP address range is not present in Active Directory.")]
        ERROR_DDS_RANGE_DOES_NOT_EXIST = 20088
    }

    /// <summary>
    /// Helper class for controlling DHCPSAPI error codes and their description strings
    /// </summary>
    internal static class DhcpsapiError
    {
        /// <summary>
        /// Gets description string based on error code
        /// </summary>
        /// <param name="e">Error code in DHCPSAPIErrorType enumeration</param>
        /// <returns>Error description string</returns>
        public static String GetDesc(DhcpsapiErrorType e)
        {
            System.Reflection.FieldInfo EnumInfo = e.GetType().GetField(e.ToString());
            System.ComponentModel.DescriptionAttribute[] EnumAttributes = 
                (System.ComponentModel.DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
 
            if (EnumAttributes.Length > 0) { return EnumAttributes[0].Description; }

            return "DHCPSAPI Error Unknown";
        }
    }

    #endregion 

}

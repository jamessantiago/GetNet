
using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;

namespace Dwo.Interop
{
    /// <summary>
    /// Contains external unmanaged code functions from DHCPSAPI Windows C library.  This class is part
    /// of the platform invoke (p/invoke) wrapper used in the Dwo .NET Library.
    /// </summary>
    /// <seealso href="http://msdn2.microsoft.com/en-us/library/26thfadc(VS.80).aspx">Consuming Unmanaged DLL Functions [MSDN]</seealso>
    internal static class NativeMethods
    {

        #region DhcpOptionFunctions...
        /// <summary>
        /// Returns an enumerated list of DHCP options for a given class and/or vendor. 
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="Flags">Use 0 when no vendor</param>
        /// <param name="ClassName">Option class name, use null for Dhcp</param>
        /// <param name="VendorName">Vendor name, use null when flag is 0</param>
        /// <param name="ResumeHandle">Pointer to start of next iteration</param>
        /// <param name="PreferredMaximum"></param>
        /// <param name="Options">Pointer to <see cref="DHCP_OPTION_VALUE_ARRAY"/></param>
        /// <param name="OptionsRead">Number of values read on one call</param>
        /// <param name="OptionsTotal">Total number of values left to be read on server</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/bb871556(VS.85).aspx">DhcpEnumOptionsV6 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpEnumOptionsV5(
            string ServerIpAddress,
            UInt32 Flags,
            string ClassName,
            string VendorName,
            ref UInt32 ResumeHandle,
            UInt32 PreferredMaximum,
            out IntPtr Options,
            out UInt32 OptionsRead,
            out UInt32 OptionsTotal);

        /// <summary>
        /// Returns information on a specific DHCP option. 
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="Flags">Use 0 when no vendor</param>
        /// <param name="OptionID">Option ID, see <a href="http://tools.ietf.org/html/rfc2132">rfc2132</a></param>
        /// <param name="ClassName">Option class name, use null for Dhcp</param>
        /// <param name="VendorName">Vendor name, use null when flag is 0</param>
        /// <param name="OptionInfo">Pointer to <see cref="DHCP_OPTION"/></param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/bb871563(VS.85).aspx">DhcpGetOptionInfoV6 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpGetOptionInfoV5(
            string ServerIpAddress,
            UInt32 Flags,
            UInt32 OptionID,
            string ClassName,
            string VendorName,
            out IntPtr OptionInfo);
        #endregion

        #region DhcpOptionValueFunctions...

        /// <summary>
        /// Returns an enumerated list of option values (the option data and the associated ID number) for a specific scope within a given class.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="Flags">Use 0 when no vendor</param>
        /// <param name="ClassName">Option class name, use null for Dhcp</param>
        /// <param name="VendorName">Vendor name, use null when flag is 0</param>
        /// <param name="ScopeInfo">Information identifying scope (server, subnet, or reservation) to be enumerated</param>
        /// <param name="ResumeHandle">Pointer to start of next iteration</param>
        /// <param name="PreferredMaximum">Max bytes returned from one call</param>
        /// <param name="OptionValues">Pointer to <see cref="DHCP_OPTION_VALUE_ARRAY"/></param>
        /// <param name="OptionsRead">Number of values read on one call</param>
        /// <param name="OptionsTotal">Total number of values left to be read on server</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/bb871557(VS.85).aspx">DhcpEnumOptionValuesV6 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpEnumOptionValuesV5(
            string ServerIpAddress,
            UInt32 Flags,
            string ClassName,
            string VendorName,
            ref DHCP_OPTION_SCOPE_INFO ScopeInfo,
            ref UInt32 ResumeHandle,
            UInt32 PreferredMaximum,
            out IntPtr OptionValues,
            out UInt32 OptionsRead,
            out UInt32 OptionsTotal);

        /// <summary>
        /// Removes an option value from a scope defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="Flags">Use 0 when no vendor</param>
        /// <param name="OptionID">Option ID, see <a href="http://tools.ietf.org/html/rfc2132">rfc2132</a></param>
        /// <param name="ClassName">Option class name, use null for Dhcp</param>
        /// <param name="VendorName">Vendor name, use null when flag is 0</param>
        /// <param name="ScopeInfo">Information identifying scope (server, subnet, or reservation)</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa813377(VS.85).aspx">DhcpRemoveOptionValueV5 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpRemoveOptionValueV5(
            string ServerIpAddress,
            UInt32 Flags,
            UInt32 OptionID,
            string ClassName,
            string VendorName,
            ref DHCP_OPTION_SCOPE_INFO ScopeInfo);

        /// <summary>
        /// Sets information for a specific option value on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="Flags">Use 0 when no vendor</param>
        /// <param name="OptionID">Option ID, see <a href="http://tools.ietf.org/html/rfc2132">rfc2132</a></param>
        /// <param name="ClassName">Option class name, use null for Dhcp</param>
        /// <param name="VendorName">Vendor name, use null when flag is 0</param>
        /// <param name="ScopeInfo">Information identifying scope (server, subnet, or reservation)</param>
        /// <param name="OptionValue">Option data values to be assigned to OptionID</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363302(VS.85).aspx">DhcpSetOptionValueV5 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpSetOptionValueV5(
            string ServerIpAddress,
            UInt32 Flags,
            UInt32 OptionID,
            string ClassName,
            string VendorName,
            ref DHCP_OPTION_SCOPE_INFO ScopeInfo,
            ref DHCP_OPTION_DATA OptionValue);

        /// <summary>
        /// Retrieves a DHCP option value (the option code and associated data) for a particular scope.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="Flags"></param>
        /// <param name="OptionID"></param>
        /// <param name="ClassName"></param>
        /// <param name="VendorName"></param>
        /// <param name="ScopeInfo"></param>
        /// <param name="OptionValue"></param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363287(VS.85).aspx">DhcpGetOptionValue [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpGetOptionValueV5(
            string ServerIpAddress,
            UInt32 Flags,
            UInt32 OptionID,
            string ClassName,
            string VendorName,
            ref DHCP_OPTION_SCOPE_INFO ScopeInfo,
            out IntPtr OptionValue);

        #endregion

        #region DhcpSubnetElementFunctions...
        /// <summary>
        /// Removes an element from a subnet defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="RemoveElementInfo">Subnet element to be removed</param>
        /// <param name="ForceFlag">Full force or otherwise</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363297(VS.85).aspx">DhcpRemoveSubnetElementV5 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpRemoveSubnetElementV5(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            ref DHCP_SUBNET_ELEMENT_DATA_V5 RemoveElementInfo,
            DHCP_FORCE_FLAG ForceFlag);

        /// <summary>
        /// Adds an element describing a feature or aspect of the subnet to the subnet entry in the DHCP database.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="AddElementInfo">Subnet element to add</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363270(VS.85).aspx">DhcpAddSubnetElementV5 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpAddSubnetElementV5(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            ref DHCP_SUBNET_ELEMENT_DATA_V5 AddElementInfo);

        /// <summary>
        /// Returns an enumerated list of elements for a specific DHCP subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="EnumElementType">Type of elements to enumerate</param>
        /// <param name="ResumeHandle">Pointer to start of next iteration</param>
        /// <param name="PreferredMaximum">Max number of bytes returned in one call</param>
        /// <param name="EnumElementInfo">Pointer to <see cref="DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5"/></param>
        /// <param name="ElementsRead">Number of items read on one call</param>
        /// <param name="ElementsTotal">Total items left to be read on server</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa813376(VS.85).aspx">DhcpEnumSubnetElementsV5 [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpEnumSubnetElementsV5(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            DHCP_SUBNET_ELEMENT_TYPE_V5 EnumElementType,
            ref UInt32 ResumeHandle,
            UInt32 PreferredMaximum,
            out IntPtr EnumElementInfo,
            out UInt32 ElementsRead,
            out UInt32 ElementsTotal);
        #endregion

        #region DhcpClientFunctions...
        /// <summary>
        /// Deletes a client information record from the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SearchInfo">Search parameters to delete by</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363281(VS.85).aspx">DhcpDeleteClientInfo [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpDeleteClientInfo(
            string ServerIpAddress,
            ref DHCP_SEARCH_INFO SearchInfo);

        /// <summary>
        /// Returns information about a specific DHCP client.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SearchInfo">Search parameters</param>
        /// <param name="ClientInfo">Pointer to <see cref="DHCP_CLIENT_INFO"/></param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363286(VS.85).aspx">DhcpGetClientInfo [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpGetClientInfo(
              String ServerIpAddress,
              ref DHCP_SEARCH_INFO SearchInfo,
              out IntPtr ClientInfo);

        /// <summary>
        /// Sets information on a client whose IP address lease is administrated by the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="ClientInfo">Client info to set (update)</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363301(VS.85).aspx">DhcpSetClientInfo [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpSetClientInfo(
            string ServerIpAddress,
            ref DHCP_CLIENT_INFO ClientInfo);

        /// <summary>
        /// Creates a client information record on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="ClientInfo">Client info to create</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363277(VS.85).aspx">DhcpCreateClientInfo [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpCreateClientInfo(
            string ServerIpAddress,
            ref DHCP_CLIENT_INFO ClientInfo);

        /// <summary>
        /// Returns an enumerated list of clients with served IP addresses in the specified subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="ResumeHandle">Pointer to start of next iteration</param>
        /// <param name="PreferredMaximum">Max number of bytes returned in one call</param>
        /// <param name="ClientInfo">Pointer to <see cref="DHCP_CLIENT_INFO_ARRAY"/></param>
        /// <param name="ElementsRead">Number of items read on one call</param>
        /// <param name="ElementsTotal">Total items left to be read on server</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363284(VS.85).aspx">DhcpEnumSubnetClients [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpEnumSubnetClients(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            ref UInt32 ResumeHandle,
            UInt32 PreferredMaximum,
            out IntPtr ClientInfo,
            out UInt32 ElementsRead,
            out UInt32 ElementsTotal);

        #endregion

        #region DhcpSubnetFunctions...
        /// <summary>
        /// Sets information about a subnet defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="SubnetInfo">Subnet info to update</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa813379(VS.85).aspx">DhcpSetSubnetInfo [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpSetSubnetInfo(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            ref DHCP_SUBNET_INFO SubnetInfo);

        /// <summary>
        /// Deletes a subnet from the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="ForceFlag">Full force or otherwise</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa813370(VS.85).aspx">DhcpDeleteSubnet [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpDeleteSubnet(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            DHCP_FORCE_FLAG ForceFlag);

        /// <summary>
        /// Creates a new subnet on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="SubnetInfo">Subnet info to create</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa813368(VS.85).aspx">DhcpCreateSubnet [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpCreateSubnet(
            string ServerIpAddress,
            UInt32 SubnetAddress,
            ref DHCP_SUBNET_INFO SubnetInfo);

        /// <summary>
        /// Returns an enumerated list of subnets defined on the DHCP server.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="ResumeHandle">Pointer to start of next iteration</param>
        /// <param name="PreferredMaximum">Max number of subnets returned in one call</param>
        /// <param name="EnumInfo">Pointer to <see cref="DHCP_IP_ARRAY"/></param>
        /// <param name="ElementsRead">Number of items read on one call</param>
        /// <param name="ElementsTotal">Total items left to be read on server</param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363285(VS.85).aspx">DhcpEnumSubnets [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpEnumSubnets(
            string ServerIpAddress,
            ref UInt32 ResumeHandle,
            UInt32 PreferredMaximum,
            out IntPtr EnumInfo,
            out UInt32 ElementsRead,
            out UInt32 ElementsTotal);

        /// <summary>
        /// Returns information on a specific subnet.
        /// </summary>
        /// <param name="ServerIpAddress">Server IP address in string form</param>
        /// <param name="SubnetAddress">Subnet network IP in uint form</param>
        /// <param name="SubnetInfo">Pointer to <see cref="DHCP_SUBNET_INFO"/></param>
        /// <returns>UInt which can be mapped to <see cref="DhcpsapiErrorType"/></returns>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363289(VS.85).aspx">DhcpGetSubnetInfo [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
        internal static extern UInt32 DhcpGetSubnetInfo(
              string ServerIpAddress,
              UInt32 SubnetAddress,
              out IntPtr SubnetInfo);
        #endregion

        #region DhcpMisc...

        /// <summary>
        /// Frees a block of buffer space returned as a parameter by the Remote Procedure Call (RPC) service.
        /// </summary>
        /// <param name="BuffPtr">Pointer to data returned by a DHCPSAPI function</param>
        /// <remarks>
        /// <p>Based on <a href="http://msdn2.microsoft.com/en-us/library/aa363299(VS.85).aspx">DhcpRpcFreeMemory [MSDN]</a></p>
        /// <h5>Interop Attributes</h5>
        /// <code>[DllImport("dhcpsapi.dll")]</code>
        /// </remarks>
        [DllImport("dhcpsapi.dll")]
        internal static extern void DhcpRpcFreeMemory(IntPtr BuffPtr);


		[DllImport("dhcpsapi.dll", CharSet = CharSet.Unicode)]
		internal static extern UInt32 DhcpGetMibInfoV5(
			  string ServerIpAddress,
			  out IntPtr MibInfo);
        #endregion

    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Dwo.Interop;

namespace Dwo
{
    /// <summary>
    /// Defines a DHCP Option on a DHCP Server.  See <see cref="DhcpOptionValue"/> to assign values to DHCP Options.
    /// </summary>
    public class DhcpOption
    {
        /// <summary>
        /// DHCP Option ID, see <a href="http://tools.ietf.org/html/rfc2132">rfc2132</a>
        /// </summary>
        public UInt32 OptionId;
        /// <summary>
        /// Option name
        /// </summary>
        public String Name;
        /// <summary>
        /// Option description
        /// </summary>
        public String Comment;
        /// <summary>
        /// Option data type
        /// </summary>
        public DhcpOptionDataType DataType;
        /// <summary>
        /// Is option value an array of values
        /// </summary>
        public bool isArray;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpOption() { }

        /// <summary>
        /// Enumeration all possible DHCP option on server
        /// </summary>
        /// <param name="server">Server IP address to enumerate</param>
        /// <param name="OptionClass">Class of options</param>
        /// <returns>An array of DHCP option found on server</returns>
        public static DhcpOption[] EnumAll(DhcpIpAddress server, DhcpOptionClassType OptionClass)
        {
            UInt32 Response = 0, ResumeHandle = 0;
            UInt32 nRead = 0, nTotal = 0;
            IntPtr retPtr, iPtr;
            List<DhcpOption> options = new List<DhcpOption>();
            DHCP_OPTION nativeOption;
            DHCP_OPTION_ARRAY nativeArray;
            String ClassId = DhcpOptionClassHash.ht[OptionClass] as String;


            for ( ; ; )
            {
                Response = NativeMethods.DhcpEnumOptionsV5(server.ToString(), 0, ClassId, null,
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
                DhcpOption opt;
                nativeArray = (DHCP_OPTION_ARRAY)Marshal.PtrToStructure<DHCP_OPTION_ARRAY>(retPtr);
                for (int i = 0; i < nativeArray.NumElements; ++i)
                {
                    iPtr = (IntPtr)(nativeArray.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION>()));
                    nativeOption = (DHCP_OPTION)Marshal.PtrToStructure<DHCP_OPTION>(iPtr);

                    opt = DhcpOption.ConvertFromNative(nativeOption);
                    if(opt != null)
                        options.Add(opt);
                }

                //free on last successful call
                if (Response == 0)
                    NativeMethods.DhcpRpcFreeMemory(retPtr);
            }

            return options.ToArray();
        }

        /// <summary>
        /// Convert a native DHCP_OPTION to its SMO form
        /// </summary>
        /// <param name="nativeOption">Native option to convert</param>
        /// <returns>SMO DHCP option</returns>
        internal static DhcpOption ConvertFromNative(DHCP_OPTION nativeOption)
        {
            DhcpOption option = new DhcpOption();

            //unsupported options
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(
               nativeOption.DefaultValue.Elements);

            if(element.OptionType == DHCP_OPTION_DATA_TYPE.DhcpBinaryDataOption ||
                element.OptionType == DHCP_OPTION_DATA_TYPE.DhcpEncapsulatedDataOption ||
                element.OptionType == DHCP_OPTION_DATA_TYPE.DhcpIpv6AddressOption)
                    return null;
        
            option.DataType = (DhcpOptionDataType)element.OptionType;
            option.OptionId = nativeOption.OptionID;
            option.Name = nativeOption.OptionName;
            option.Comment = nativeOption.OptionComment;
            option.isArray = nativeOption.OptionType == DHCP_OPTION_TYPE.DhcpArrayTypeOption ? true : false;

            return option;
        }

        //public static void ValidateOptionValue(DhcpIpAddress Server, DhcpOptionValue OptValue)
        //{
        //    //special case option 81 (DNS)
        //    if (OptValue.OptionId == 81 || OptValue.OptionId == 51)
        //    {
        //        if (OptValue.OptionData.Type != DhcpOptionDataType.DWordType)
        //            throw new DhcpException(1629);
        //        else
        //            return;
        //    }

        //    UInt32 Response = 0;
        //    IntPtr InfoPtr;

        //    Response = NativeMethods.DhcpGetOptionInfoV5(Server.ToString(), 0, OptValue.OptionId, DhcpOptionClassHash.ht[OptValue.ClassType] as String, null, out InfoPtr);

        //    if (Response != 0)
        //    {
        //        if (InfoPtr != IntPtr.Zero)
        //            NativeMethods.DhcpRpcFreeMemory(InfoPtr);

        //        throw new DhcpException(Response);
        //    }
        //    DHCP_OPTION option = (DHCP_OPTION)Marshal.PtrToStructure(InfoPtr, typeof(DHCP_OPTION));

        //    //Get Data Type
        //    DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure(option.DefaultValue.Elements, typeof(DHCP_OPTION_DATA_ELEMENT));
        //    DHCP_OPTION_DATA_TYPE DhcpDataType = element.OptionType;

        //    //Free RPC Call
        //    NativeMethods.DhcpRpcFreeMemory(InfoPtr);

        //    //Compare Data Type
        //    //ERROR_DATATYPE_MISMATCH = 1629
        //    switch (OptValue.OptionData.Type)
        //    {
        //        case DhcpOptionDataType.ByteType:
        //            if (DhcpDataType != DHCP_OPTION_DATA_TYPE.DhcpByteOption)
        //                throw new DhcpException(1629);


        //            break;
        //        case DhcpOptionDataType.WordType:
        //            if (DhcpDataType != DHCP_OPTION_DATA_TYPE.DhcpWordOption)
        //                throw new DhcpException(1629);


        //            break;
        //        case DhcpOptionDataType.DWordType:
        //            if (DhcpDataType != DHCP_OPTION_DATA_TYPE.DhcpDWordOption)
        //                throw new DhcpException(1629);


        //            break;
        //        case DhcpOptionDataType.DWordDWordType:
        //            if (DhcpDataType != DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption)
        //                throw new DhcpException(1629);


        //            break;
        //        case DhcpOptionDataType.IpAddressType:
        //            if (DhcpDataType != DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption)
        //                throw new DhcpException(1629);


        //            break;
        //        case DhcpOptionDataType.StringDataType:
        //            if (DhcpDataType != DHCP_OPTION_DATA_TYPE.DhcpStringDataOption)
        //                throw new DhcpException(1629);


        //            break;
        //        default:
        //            throw new NotImplementedException("OptionData Type: " + DhcpDataType);
        //    }

        //    //Check if Array
        //    //if (option.OptionType == DHCP_OPTION_TYPE.DhcpUnaryElementTypeOption && isArray)
        //    //    throw new Exception("OptionValue data should be single value");

        //    //if (option.OptionType == DHCP_OPTION_TYPE.DhcpArrayTypeOption && !isArray)
        //    //    throw new Exception("OptionValue data should be an array");

        //}
    }

    /// <summary>
    /// Option class types
    /// </summary>
    public enum DhcpOptionClassType
    {
        /// <summary>
        /// DHCP standard option class
        /// </summary>
        Dhcp,
        /// <summary>
        /// BOOTP standard option class
        /// </summary>
        Bootp,
        /// <summary>
        /// Routing and Remote Access standard option class
        /// </summary>
        Routing
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

using Dwo.Interop;

namespace Dwo
{

    /// <summary>
    /// Defines a DHCP Option's value(s).  See <see cref="DhcpOption"/> for DHCP Option definitions.
    /// </summary>
    public class DhcpOptionValue
    {
        /// <summary>
        /// DHCP Option ID, see <a href="http://tools.ietf.org/html/rfc2132">rfc2132</a>
        /// </summary>
        public UInt32 OptionId;
        /// <summary>
        /// DHCP option class (Dhcp, Bootp, Routing)
        /// </summary>
        public DhcpOptionClassType ClassType;
        /// <summary>
        /// Option data value(s)
        /// </summary>
        public DhcpOptionData OptionData;
        /// <summary>
        /// Scope option value belongs to (Server, Subnet, Reservation)
        /// </summary>
        public DhcpOptionScope OptionScope;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpOptionValue() 
        {
            this.OptionId = UInt32.MaxValue;
            this.ClassType = DhcpOptionClassType.Dhcp;
        }

        /// <summary>
        /// Sets DHCP option value on server
        /// </summary>
        /// <remarks>Set() creates or updates value.  All fields must be set before calling Set()</remarks>
        /// <param name="Server">Server IP</param>
        public void Set(DhcpIpAddress Server)
        {
            if (this.OptionId == UInt32.MaxValue || this.OptionScope == null)
                throw new InvalidOperationException("Invalid Option Data");

            //Run Check to make sure Id exist and types (data, isArray) are correct
            //DhcpOption.ValidateOptionValue(Server, this);

            //Get Option Class
            String ClassId = DhcpOptionClassHash.ht[this.ClassType] as String;

            //Get Scope Setting
            DHCP_OPTION_SCOPE_INFO NativeScopeInfo = this.OptionScope.ConvertToNative();

            uint Response = 0;
            using (MemManager Mem = new MemManager())
            {
                DHCP_OPTION_DATA NativeOptionData = this.OptionData.ConvertToNative(Mem);

                Response = NativeMethods.DhcpSetOptionValueV5(Server.ToString(), 0, this.OptionId, ClassId, null, ref NativeScopeInfo, ref NativeOptionData);
            }

            if (Response != 0)
                throw new DhcpException(Response);

        }

        /// <summary>
        /// Removes DHCP option from server
        /// </summary>
        /// <remarks>Remove all values from option.  OptionId, OptionScope, and ClassType must be set before calling Remove()</remarks>
        /// <param name="Server">Server IP</param>
        public void Remove(DhcpIpAddress Server)
        {
            if (this.OptionId == UInt32.MaxValue || this.OptionScope == null)
                throw new InvalidOperationException("Invalid Option Data");

            String ClassId = DhcpOptionClassHash.ht[this.ClassType] as String;

            //Get Scope Setting
            DHCP_OPTION_SCOPE_INFO NativeScopeInfo = this.OptionScope.ConvertToNative();

            uint Response = NativeMethods.DhcpRemoveOptionValueV5(Server.ToString(), 0, this.OptionId, ClassId, null, ref NativeScopeInfo);

            if (Response != 0)
                throw new DhcpException(Response);
        }

        /// <summary>
        /// Get DHCP option value from server
        /// </summary>
        /// <remarks>OptionId, OptionScope, and ClassType must be set before calling Get().  OptionData will be set after call</remarks>
        /// <param name="Server">Server IP</param>
        public void Get(DhcpIpAddress Server)
        {
            if (this.OptionId == UInt32.MaxValue || this.OptionScope == null)
                throw new InvalidOperationException("Invalid Option Data");

            String ClassId = DhcpOptionClassHash.ht[this.ClassType] as String;

            //Get Scope Setting
            DHCP_OPTION_SCOPE_INFO NativeScopeInfo = this.OptionScope.ConvertToNative();

            IntPtr iPtr = IntPtr.Zero;
            uint Response = NativeMethods.DhcpGetOptionValueV5(Server.ToString(), 0, this.OptionId, ClassId, null, ref NativeScopeInfo, out iPtr);

            if (Response != 0)
            {
                if (iPtr != IntPtr.Zero)
                    NativeMethods.DhcpRpcFreeMemory(iPtr);

                throw new DhcpException(Response);
            }

            //
            DHCP_OPTION_VALUE NativeOptVal = (DHCP_OPTION_VALUE)Marshal.PtrToStructure<DHCP_OPTION_VALUE>(iPtr);

            this.OptionData = DhcpOptionData.CovertFromNative(NativeOptVal.Value);
           

        }

        /// <summary>
        /// Enumerates all option values on server based on scope and class 
        /// </summary>
        /// <param name="Server">Server IP</param>
        /// <param name="OptionClass">Class type to enumerate</param>
        /// <param name="Scope">Defined scope to enumerate</param>
        /// <returns>An array of DHCP option values</returns>
        public static DhcpOptionValue[] EnumAll(DhcpIpAddress Server, DhcpOptionClassType OptionClass, DhcpOptionScope Scope)
        {
            DHCP_OPTION_VALUE NativeOptionValue;
            IntPtr retPtr, iPtr;
            DHCP_OPTION_VALUE_ARRAY NativeOptVArray;
            List<DhcpOptionValue> ovList = new List<DhcpOptionValue>();
            DhcpOptionValue  ov;

            //Get Scope Setting
            DHCP_OPTION_SCOPE_INFO NativeScopeInfo = Scope.ConvertToNative();

            String ClassId = DhcpOptionClassHash.ht[OptionClass] as String;

            UInt32 rHandle = 0, OptionsRead = 0, OptionsTotal = 0, Response = 0;
            for ( ; ; )
            {
                Response = NativeMethods.DhcpEnumOptionValuesV5(Server.ToString(), 0, ClassId, null, ref NativeScopeInfo, ref rHandle,
                    65536, out retPtr, out OptionsRead, out OptionsTotal);

                //ERROR_NO_MORE_ITEMS
                if (Response == 259)
                    break;

                //ERROR_MORE_DATA = 234
                if(Response != 0 && Response != 234 || retPtr == IntPtr.Zero)
                {
                    if(retPtr != IntPtr.Zero)
                        NativeMethods.DhcpRpcFreeMemory(retPtr);
                    throw new DhcpException(Response);
                }

                NativeOptVArray = (DHCP_OPTION_VALUE_ARRAY)Marshal.PtrToStructure<DHCP_OPTION_VALUE_ARRAY>(retPtr);
                for (int i = 0; i < NativeOptVArray.NumElements; ++i)
                {
                    iPtr = (IntPtr)(NativeOptVArray.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_VALUE>()));
                    NativeOptionValue = (DHCP_OPTION_VALUE)Marshal.PtrToStructure<DHCP_OPTION_VALUE>(iPtr);

                    ov = new DhcpOptionValue();
                    ov.ClassType = OptionClass;
                    ov.OptionScope = Scope;
                    ov.OptionId = NativeOptionValue.OptionID;
                    ov.OptionData = DhcpOptionData.CovertFromNative(NativeOptionValue.Value);

                    ovList.Add(ov);

                }

                //free on last successful call
                if (Response == 0)
                    NativeMethods.DhcpRpcFreeMemory(retPtr);
            }

            return ovList.ToArray();
        }
    }

    /// <summary>
    /// Defines an assignment scope for DHCP options
    /// </summary>
    public class DhcpOptionScope
    {
        /// <summary>
        /// Type of scope (Server, Subnet, Reservation)
        /// </summary>
        public DhcpOptionScopeType type;
        /// <summary>
        /// Subnet network IP address (used if subnet or reservation type)
        /// </summary>
        public DhcpIpAddress SubnetIp;
        /// <summary>
        /// Reserveration IP address (used if reservation type)
        /// </summary>
        public DhcpIpAddress ReservationIp;

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpOptionScope() 
        {
            SubnetIp = null;
            ReservationIp = null;
        }
    
        /// <summary>
        /// Convert this object to its native form
        /// </summary>
        /// <returns>A native scope info</returns>
        internal DHCP_OPTION_SCOPE_INFO ConvertToNative()
        {
            DHCP_OPTION_SCOPE_INFO sInfo = new DHCP_OPTION_SCOPE_INFO();
            switch (this.type)
            {
                case DhcpOptionScopeType.Server:
                    sInfo.ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpGlobalOptions;
                    sInfo.GlobalScopeInfo = IntPtr.Zero;
                    break;
                case DhcpOptionScopeType.Subnet:
                    sInfo.ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpSubnetOptions;
                    if (this.SubnetIp == null)
                        throw new InvalidOperationException("Subnet IP not assigned");
                    sInfo.SubnetScopeInfo = this.SubnetIp.GetUIntAddress();
                    break;
                case DhcpOptionScopeType.Reservation:
                    sInfo.ScopeType = DHCP_OPTION_SCOPE_TYPE.DhcpReservedOptions;
                    if (this.SubnetIp == null)
                        throw new InvalidOperationException("Subnet IP not assigned");
                    if (this.ReservationIp == null)
                        throw new InvalidOperationException("Reservation IP not assigned");
                    sInfo.ReservedScopeInfo = new DHCP_RESERVED_SCOPE();
                    sInfo.ReservedScopeInfo.ReservedIpSubnetAddress = this.SubnetIp.GetUIntAddress();
                    sInfo.ReservedScopeInfo.ReservedIpAddress = this.ReservationIp.GetUIntAddress();
                    break;
            }
            return sInfo;
        }
    }

    #region OptionData...
    /// <summary>
    /// Base class for all DhcpOptionData* types
    /// </summary>
    public abstract class DhcpOptionData
    {
        /// <summary>
        /// Data type
        /// </summary>
        public DhcpOptionDataType Type;
        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpOptionData() { }

        /// <summary>
        /// Create base option data and type
        /// </summary>
        /// <param name="type">Type of data</param>
        public DhcpOptionData(DhcpOptionDataType type)
        {

            this.Type = type;
        }

        /// <summary>
        /// Each OptionData* converts their own data type of a native type
        /// </summary>
        /// <param name="Mem">Tracks memory allocations</param>
        /// <returns>A native DHCP option data</returns>
        internal abstract DHCP_OPTION_DATA ConvertToNative(MemManager Mem);

        /// <summary>
        /// Converts a native option data to its SMO representation based on type
        /// </summary>
        /// <param name="NativeOptData">Native option data to be converted</param>
        /// <returns>SMO option data</returns>
        internal static DhcpOptionData CovertFromNative(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);
            switch (element.OptionType)
            {
                case DHCP_OPTION_DATA_TYPE.DhcpByteOption:
                    return new DhcpOptionDataByte(NativeOptData);
                case DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption:
                    return new DhcpOptionDataDwordDword(NativeOptData);
                case DHCP_OPTION_DATA_TYPE.DhcpDWordOption:
                    return new DhcpOptionDataDword(NativeOptData);
                case DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption:
                    return new DhcpOptionDataIp(NativeOptData);
                case DHCP_OPTION_DATA_TYPE.DhcpStringDataOption:
                    return new DhcpOptionDataString(NativeOptData);
                case DHCP_OPTION_DATA_TYPE.DhcpWordOption:
                    return new DhcpOptionDataWord(NativeOptData);
                default:
                    throw new NotImplementedException("OptionData Type: " + element.OptionType);
            }
        }

    }

    /// <summary>
    /// Defines DHCP option data of byte type
    /// </summary>
    public class DhcpOptionDataByte : DhcpOptionData
    {
        /// <summary>
        /// Array of byte data
        /// </summary>
        public Byte[] Data;
        /// <summary>
        /// Creates null data
        /// </summary>
        public DhcpOptionDataByte() { this.Type = DhcpOptionDataType.ByteType; }
        /// <summary>
        /// Sets Data with bytes
        /// </summary>
        /// <param name="bytes">Byte array to set data with</param>
        public DhcpOptionDataByte(Byte[] bytes)
            : base(DhcpOptionDataType.ByteType)
        {
            this.Data = bytes;
        }

        /// <summary>
        /// Create object using native option data
        /// </summary>
        /// <param name="NativeOptData">Native DHCP option data</param>
        internal DhcpOptionDataByte(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            this.Type = DhcpOptionDataType.ByteType;

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);

            if (element.OptionType != DHCP_OPTION_DATA_TYPE.DhcpByteOption)
                throw new ArgumentException("NativeOptData type not Byte");

            this.Data = new Byte[NativeOptData.NumElements];

            this.Data[0] = element.ByteOption;

            IntPtr iPtr;
            for (int i = 1; i < NativeOptData.NumElements; ++i)
            {
                iPtr = (IntPtr)(NativeOptData.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(iPtr);
                this.Data[i] = element.ByteOption;
            }
        }

        /// <summary>
        /// Converts this object to native option data
        /// </summary>
        /// <param name="Mem">Tracks memory allocations to free later</param>
        /// <returns>A native DHCP option data</returns>
        internal override DHCP_OPTION_DATA ConvertToNative(MemManager Mem)
        {

            if (this.Data.Length == 0)
                throw new InvalidOperationException("OptionValue: No data to convert");

            DHCP_OPTION_DATA_ELEMENT element;
            DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();

            data.NumElements = (UInt32)this.Data.Length;
            data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
            IntPtr iptr;
            for (int i = 0; i < data.NumElements; i++)
            {
                element = new DHCP_OPTION_DATA_ELEMENT();
                element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpByteOption;
                element.ByteOption = this.Data[i];
                
                iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                Marshal.StructureToPtr(element, iptr, false);

            }
            return data;
        }
    }

    /// <summary>
    /// Defines DHCP option data of DhcpIpAddress type
    /// </summary>
    public class DhcpOptionDataIp : DhcpOptionData
    {
        /// <summary>
        /// Array of DhcpIpAddres data
        /// </summary>
        public DhcpIpAddress[] Data;
        /// <summary>
        /// Creates null data
        /// </summary>
        public DhcpOptionDataIp() { this.Type = DhcpOptionDataType.IpAddressType; }
        /// <summary>
        /// Sets Data with address array
        /// </summary>
        /// <param name="ips">Array of DhcpIpAddress to set Data with</param>
        public DhcpOptionDataIp(DhcpIpAddress[] ips)
            : base(DhcpOptionDataType.IpAddressType)
        {
            this.Data = ips;
        }

        /// <summary>
        /// Create object using native option data
        /// </summary>
        /// <param name="NativeOptData">Native DHCP option data</param>
        internal DhcpOptionDataIp(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            this.Type = DhcpOptionDataType.IpAddressType;

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);

            if (element.OptionType != DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption)
                throw new ArgumentException("NativeOptData type not IP");

            this.Data = new DhcpIpAddress[NativeOptData.NumElements];

            this.Data[0] = new DhcpIpAddress(element.DWordOption);

            IntPtr iPtr;
            for (int i = 1; i < NativeOptData.NumElements; ++i)
            {
                iPtr = (IntPtr)(NativeOptData.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(iPtr);
                this.Data[i] = new DhcpIpAddress(element.DWordOption);
            }
        }

        /// <summary>
        /// Converts this object to native option data and IpAdress type
        /// </summary>
        /// <param name="Mem">Tracks memory allocations to free later</param>
        /// <returns>A native DHCP option data</returns>
        internal override DHCP_OPTION_DATA ConvertToNative(MemManager Mem)
        {

            if (this.Data.Length == 0)
                throw new InvalidOperationException("OptionValue: No data to convert");

            DHCP_OPTION_DATA_ELEMENT element;
            DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();

            data.NumElements = (UInt32)this.Data.Length;
            data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
            IntPtr iptr;
            for (int i = 0; i < data.NumElements; i++)
            {
                element = new DHCP_OPTION_DATA_ELEMENT();
                element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpIpAddressOption;
                element.DWordOption = this.Data[i].GetUIntAddress();

                iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                Marshal.StructureToPtr(element, iptr, false);

            }
            return data;
        }
    }

    /// <summary>
    /// Defines DHCP option data of UInt16 type
    /// </summary>
    public class DhcpOptionDataWord : DhcpOptionData
    {
        /// <summary>
        /// Array of UInt16 data
        /// </summary>
        public UInt16[] Data;
        /// <summary>
        /// Create null data
        /// </summary>
        public DhcpOptionDataWord() { this.Type = DhcpOptionDataType.WordType; }
        /// <summary>
        /// Set Data to words array
        /// </summary>
        /// <param name="words">Array of UInt16 to set Data with</param>
        public DhcpOptionDataWord(UInt16[] words)
            : base(DhcpOptionDataType.WordType)
        {
            this.Data = words;
        }

        /// <summary>
        /// Create object using native option data
        /// </summary>
        /// <param name="NativeOptData">Native DHCP option data</param>
        internal DhcpOptionDataWord(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            this.Type = DhcpOptionDataType.WordType;

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);

            if (element.OptionType != DHCP_OPTION_DATA_TYPE.DhcpWordOption)
                throw new ArgumentException("NativeOptData type not WORD");

            this.Data = new UInt16[NativeOptData.NumElements];

            this.Data[0] = element.WordOption;

            IntPtr iPtr;
            for (int i = 1; i < NativeOptData.NumElements; ++i)
            {
                iPtr = (IntPtr)(NativeOptData.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(iPtr);
                this.Data[i] = element.WordOption;
            }
        }

        /// <summary>
        /// Converts this object to native option data and WORD type
        /// </summary>
        /// <param name="Mem">Tracks memory allocations to free later</param>
        /// <returns>A native DHCP option data</returns>
        internal override DHCP_OPTION_DATA ConvertToNative(MemManager Mem)
        {

            if (this.Data.Length == 0)
                throw new InvalidOperationException("OptionValue: No data to convert");

            DHCP_OPTION_DATA_ELEMENT element;
            DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();

            data.NumElements = (UInt32)this.Data.Length;
            data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
            IntPtr iptr;
            for (int i = 0; i < data.NumElements; i++)
            {
                element = new DHCP_OPTION_DATA_ELEMENT();
                element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpWordOption;
                element.WordOption = this.Data[i];

                iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                Marshal.StructureToPtr(element, iptr, false);

            }
            return data;
        }
    }

    /// <summary>
    /// Defines DHCP option data of UInt32 type
    /// </summary>
    public class DhcpOptionDataDword : DhcpOptionData
    {
        /// <summary>
        /// Array of UInt32 data
        /// </summary>
        public UInt32[] Data;
        /// <summary>
        /// Create null data
        /// </summary>
        public DhcpOptionDataDword() { this.Type = DhcpOptionDataType.DWordType; }
        /// <summary>
        /// Sets Data to dwords array
        /// </summary>
        /// <param name="dwords">Array of UInt32 to set Data with</param>
        public DhcpOptionDataDword(UInt32[] dwords)
            : base(DhcpOptionDataType.DWordType)
        {
            this.Data = dwords;
        }

        /// <summary>
        /// Create object using native option data
        /// </summary>
        /// <param name="NativeOptData">Native DHCP option data</param>
        internal DhcpOptionDataDword(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            this.Type = DhcpOptionDataType.DWordType; 

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);

            if (element.OptionType != DHCP_OPTION_DATA_TYPE.DhcpDWordOption)
                throw new ArgumentException("NativeOptData type not DWORD");

            this.Data = new UInt32[NativeOptData.NumElements];

            this.Data[0] = element.DWordOption;

            IntPtr iPtr;
            for (int i = 1; i < NativeOptData.NumElements; ++i)
            {
                iPtr = (IntPtr)(NativeOptData.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(iPtr);
                this.Data[i] = element.DWordOption;
            }
        }

        /// <summary>
        /// Converts this object to native option data and DWORD type
        /// </summary>
        /// <param name="Mem">Tracks memory allocations to free later</param>
        /// <returns>A native DHCP option data</returns>
        internal override DHCP_OPTION_DATA ConvertToNative(MemManager Mem)
        {

            if (this.Data.Length == 0)
                throw new InvalidOperationException("OptionValue: No data to convert");

            DHCP_OPTION_DATA_ELEMENT element;
            DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();

            data.NumElements = (UInt32)this.Data.Length;
            data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
            IntPtr iptr;
            for (int i = 0; i < data.NumElements; i++)
            {
                element = new DHCP_OPTION_DATA_ELEMENT();
                element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpDWordOption;
                element.DWordOption = this.Data[i];

                iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                Marshal.StructureToPtr(element, iptr, false);

            }
            return data;
        }
    }

    /// <summary>
    /// Defines DHCP option data of UInt64 type
    /// </summary>
    public class DhcpOptionDataDwordDword : DhcpOptionData
    {
        /// <summary>
        /// Array of UInt64 data
        /// </summary>
        public UInt64[] Data;
        /// <summary>
        /// Create null data
        /// </summary>
        public DhcpOptionDataDwordDword() { this.Type = DhcpOptionDataType.DWordDWordType; }
        /// <summary>
        /// Set Data to array of ddwords
        /// </summary>
        /// <param name="ddwords">Array of UInt64 to set Data with</param>
        public DhcpOptionDataDwordDword(UInt64[] ddwords)
            : base(DhcpOptionDataType.DWordDWordType)
        {
            this.Data = ddwords;
        }

        /// <summary>
        /// Create object using native option data
        /// </summary>
        /// <param name="NativeOptData">Native DHCP option data</param>
        internal DhcpOptionDataDwordDword(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            this.Type = DhcpOptionDataType.DWordDWordType;

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);

            if (element.OptionType != DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption)
                throw new ArgumentException("NativeOptData type not DWORDDWORD");

            this.Data = new UInt64[NativeOptData.NumElements];
            
            this.Data[0] = ((((UInt64)element.DWordDWordOption.UpperWord1) << 32) | element.DWordDWordOption.LowerWord2);

            IntPtr iPtr;
            for (int i = 1; i < NativeOptData.NumElements; ++i)
            {
                iPtr = (IntPtr)(NativeOptData.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(iPtr);
                this.Data[i] = ((((UInt64)element.DWordDWordOption.UpperWord1) << 32) | element.DWordDWordOption.LowerWord2); ;
            }
        }

        /// <summary>
        /// Converts this object to native option data and DWORD_DWORD type
        /// </summary>
        /// <param name="Mem">Tracks memory allocations to free later</param>
        /// <returns>A native DHCP option data</returns>
        internal override DHCP_OPTION_DATA ConvertToNative(MemManager Mem)
        {

            if (this.Data.Length == 0)
                throw new InvalidOperationException("OptionValue: No data to convert");

            DHCP_OPTION_DATA_ELEMENT element;
            DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();

            data.NumElements = (UInt32)this.Data.Length;
            data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
            IntPtr iptr;
            for (int i = 0; i < data.NumElements; i++)
            {
                element = new DHCP_OPTION_DATA_ELEMENT();
                element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpDWordDWordOption;
                element.DWordDWordOption.LowerWord2 = (UInt32)(this.Data[i] & 0xFFFFFFFF);
                element.DWordDWordOption.UpperWord1 = (UInt32)((this.Data[i] & 0xFFFFFFFF00000000) >> 32);

                iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                Marshal.StructureToPtr(element, iptr, false);

            }
            return data;
        }
    }

    /// <summary>
    /// Defines DHCP option data of string type
    /// </summary>
    public class DhcpOptionDataString : DhcpOptionData
    {
        /// <summary>
        /// Array of String data
        /// </summary>
        public String[] Data;
        /// <summary>
        /// Create null data
        /// </summary>
        public DhcpOptionDataString() { this.Type = DhcpOptionDataType.StringDataType; }
        /// <summary>
        /// Sets Data to array of Strings
        /// </summary>
        /// <param name="strings">Array of Stings to set Data with</param>
        public DhcpOptionDataString(String[] strings)
            : base(DhcpOptionDataType.StringDataType)
        {
            this.Data = strings;
        }

        /// <summary>
        /// Create object using native option data
        /// </summary>
        /// <param name="NativeOptData">Native DHCP option data</param>
        internal DhcpOptionDataString(DHCP_OPTION_DATA NativeOptData)
        {
            if (NativeOptData.NumElements == 0 || NativeOptData.Elements == IntPtr.Zero)
                throw new ArgumentException("DHCP_OPTION_DATA not valid");

            this.Type = DhcpOptionDataType.StringDataType;

            //grab first element for type test
            DHCP_OPTION_DATA_ELEMENT element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(NativeOptData.Elements);

            if (element.OptionType != DHCP_OPTION_DATA_TYPE.DhcpStringDataOption)
                throw new ArgumentException("NativeOptData type not String");

            this.Data = new String[NativeOptData.NumElements];

            this.Data[0] = Marshal.PtrToStringUni(element.StringOption);

            IntPtr iPtr;
            for (int i = 1; i < NativeOptData.NumElements; ++i)
            {
                iPtr = (IntPtr)(NativeOptData.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                element = (DHCP_OPTION_DATA_ELEMENT)Marshal.PtrToStructure<DHCP_OPTION_DATA_ELEMENT>(iPtr);
                this.Data[i] = Marshal.PtrToStringUni(element.StringOption);
            }
        }

        /// <summary>
        /// Converts this object to native option data and String type
        /// </summary>
        /// <param name="Mem">Tracks memory allocations to free later</param>
        /// <returns>A native DHCP option data</returns>
        internal override DHCP_OPTION_DATA ConvertToNative(MemManager Mem)
        {

            if (this.Data.Length == 0)
                throw new InvalidOperationException("OptionValue: No data to convert");

            DHCP_OPTION_DATA_ELEMENT element;
            DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();

            data.NumElements = (UInt32)this.Data.Length;
            data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
            IntPtr iptr;
            for (int i = 0; i < data.NumElements; i++)
            {
                element = new DHCP_OPTION_DATA_ELEMENT();
                element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpStringDataOption;
                element.StringOption = Mem.AllocString(this.Data[i]);

                iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf<DHCP_OPTION_DATA_ELEMENT>()));
                Marshal.StructureToPtr(element, iptr, false);

            }
            return data;
        }
    }
    #endregion

    /// <summary>
    /// Helper class to convert SMO enum option class types to native type strings
    /// </summary>
    static internal class DhcpOptionClassHash
    {
        /// <summary>
        /// Mapping table
        /// </summary>
        public static Hashtable ht = new Hashtable();
        /// <summary>
        /// Static constructor
        /// </summary>
        static DhcpOptionClassHash()
        {
            ht.Add(DhcpOptionClassType.Dhcp, null);
            ht.Add(DhcpOptionClassType.Bootp, "Default BOOTP Class");
            ht.Add(DhcpOptionClassType.Routing, "Default Routing and Remote Access Class");
        }
    }

    #region Enums...
    /// <summary>
    /// DHCP option data types
    /// </summary>
    /// <remarks>BinaryDataType, EncapsulatedDataType, and Ipv6AddressType not implemented</remarks>
    public enum DhcpOptionDataType
    {
        /// <summary>
        /// Option data of byte type
        /// </summary>
        ByteType,
        /// <summary>
        /// Option data of UInt16 type
        /// </summary>
        WordType,
        /// <summary>
        /// Option data of UInt32 type
        /// </summary>
        DWordType,
        /// <summary>
        /// Option data of UInt64 type
        /// </summary>
        DWordDWordType,
        /// <summary>
        /// Option data of DhcpIpAddress type
        /// </summary>
        IpAddressType,
        /// <summary>
        /// Option data of string type
        /// </summary>
        StringDataType
     //   BinaryDataType,
     //   EncapsulatedDataType,
     //   Ipv6AddressType
    }

    /// <summary>
    /// DHCP option scope types
    /// </summary>
    public enum DhcpOptionScopeType
    {
        /// <summary>
        /// Server options
        /// </summary>
        Server,
        /// <summary>
        /// Subnet options
        /// </summary>
        Subnet,
        /// <summary>
        /// Reservation options 
        /// </summary>
        Reservation
    }
    #endregion
}


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace Dwo.Interop
{
    /// <summary>
    /// Helper class to manage heap memory allocations used by unmanaged function calls via Platform Invoke
    /// </summary>
    internal class MemManager : IDisposable
    {
        /// <summary>
        /// Allocates, copies and tracks memory needed to marshal a structure to the unmanaged memory heap
        /// </summary>
        /// <param name="structure">Structure to allocate memory for and copy to that allocation</param>
        /// <returns>Pointer to allocated memory based on structure</returns>
        /// <example>
        /// <code>
        /// using (MemManager mem = new MemManager())
        /// {
        ///     DHCP_IP_RESERVATION_V4 rip = new DHCP_IP_RESERVATION_V4();
        ///     ...   
        ///     Byte[] mac = /*MAC BYTES*/;
        ///     DHCP_CLIENT_UID uid = new DHCP_CLIENT_UID();
        ///     uid.DataLength = (uint)mac.Length;
        ///     uid.Data = Mem.AllocByteArray(mac);
        ///     rip.ReservedForClient = Mem.AllocStruct(uid);
        ///     ...
        /// }
        /// </code>
        /// </example>
        public IntPtr AllocStruct(Object structure)
        {
            if (isDisposed)
                throw new ObjectDisposedException(this.ToString());

            IntPtr iptr = Marshal.AllocHGlobal(Marshal.SizeOf<Object>(structure));
            Marshal.StructureToPtr<Object>(structure, iptr, false);
            allocations.Add(iptr);
            return iptr;
        }

        /// <summary>
        /// Allocates, copies and tracks memory needed to marshal a Byte array to the unmanaged memory heap
        /// </summary>
        /// <param name="bytes">Byte array to allocate memory for and copy to that allocation</param>
        /// <returns>Pointer to allocated memory based on Btye array</returns>
        /// <example>
        /// <code>
        /// using (MemManager mem = new MemManager())
        /// {
        ///     DHCP_IP_RESERVATION_V4 rip = new DHCP_IP_RESERVATION_V4();
        ///     ...   
        ///     Byte[] mac = /*MAC BYTES*/;
        ///     DHCP_CLIENT_UID uid = new DHCP_CLIENT_UID();
        ///     uid.DataLength = (uint)mac.Length;
        ///     uid.Data = Mem.AllocByteArray(mac);
        ///     rip.ReservedForClient = Mem.AllocStruct(uid);
        ///     ...
        /// }
        /// </code>
        /// </example>
        public IntPtr AllocByteArray(Byte[] bytes)
        {
            if (isDisposed)
                throw new ObjectDisposedException(this.ToString());

            IntPtr iptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, iptr, bytes.Length);
            allocations.Add(iptr);
            return iptr;
        }

        /// <summary>
        /// Allocates and tracks memory needed to marshal a array of type and arraysize to the unmanaged memory heap
        /// </summary>
        /// <param name="type">Type of element used in allocated array</param>
        /// <param name="arraysize">Number of items in allocated array</param>
        /// <returns>Pointer to an allocated array of type.  Size of allocation = (sizeof(type)*arraysize)</returns>
        /// <example>
        /// <code>
        /// using (MemManager mem = new MemManager())
        /// {
        ///     DHCP_OPTION_DATA_ELEMENT element;
        ///     DHCP_OPTION_DATA data = new DHCP_OPTION_DATA();
        /// 
        ///     data.NumElements = (UInt32)/*Some Length*/;
        ///     data.Elements = Mem.AllocArray(typeof(DHCP_OPTION_DATA_ELEMENT), data.NumElements);
        ///     IntPtr iptr;
        ///     for (int i = 0; i &lt; data.NumElements; i++)
        ///     {
        ///         element = new DHCP_OPTION_DATA_ELEMENT();
        ///         element.OptionType = DHCP_OPTION_DATA_TYPE.DhcpByteOption;
        ///         element.ByteOption = /*Some Byte Data*/;
        /// 
        ///         iptr = (IntPtr)(data.Elements.ToInt32() + (i * Marshal.SizeOf(typeof(DHCP_OPTION_DATA_ELEMENT))));
        ///         Marshal.StructureToPtr(element, iptr, false);
        ///     }
        /// }
        /// </code>
        /// </example>
        public IntPtr AllocArray(Type type, uint arraysize)
        {
            if (isDisposed)
                throw new ObjectDisposedException(this.ToString());

            IntPtr iptr = Marshal.AllocHGlobal((int)arraysize * Marshal.SizeOf<Type>(type));
            allocations.Add(iptr);
            return iptr;
        }

        /// <summary>
        /// Allocates, copies and tracks memory needed to marshal a string to the unmanaged memory heap
        /// </summary>
        /// <param name="str">String to allocate memory for and copy to that allocation</param>
        /// <returns>Pointer to allocated memory based on string parameter</returns>
        /// <example>
        /// <code>
        /// using (MemManager mem = new MemManager())
        /// {
        ///     ...
        ///     info.ClientName = Mem.AllocString(/*String Hostname*/);
        ///     ...
        /// }
        /// </code>
        /// </example>
        public IntPtr AllocString(String str)
        {
            if (isDisposed)
                throw new ObjectDisposedException(this.ToString());

            IntPtr iptr = Marshal.StringToHGlobalUni(str);
            allocations.Add(iptr);
            return iptr;
        }

        /// <summary>
        /// Dummy Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleaup up this object's umanaged memory allocations.
        /// </summary>
        /// <param name="disposing">Called by managed dispose?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                //clean managed
                if (disposing) { }

                //clean unmanaged
                //Console.WriteLine("Disposing ("+ allocations.Count +")...");
                foreach (IntPtr i in this.allocations)
                    Marshal.FreeHGlobal(i);
                allocations.Clear();
                
            }
            isDisposed = true;
        }

        ~MemManager() { Dispose(false); }

        /// <summary>
        /// Has this object been disposed already?
        /// </summary>
        protected bool isDisposed = false;
        /// <summary>
        /// List of all unmanaged memory allocations made by an instance of this class.  Allocations will be deleted on dispose.
        /// </summary>
        protected List<IntPtr> allocations = new List<IntPtr>();
    }
}

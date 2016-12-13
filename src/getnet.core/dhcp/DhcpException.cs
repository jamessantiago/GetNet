
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Dwo.Interop;

namespace Dwo
{
    /// <summary>
    /// Dhcp specific exception object
    /// </summary>
    public class DhcpException : Exception
    {
        private uint DhcpErrCode;
        private bool isDhcpErr = true;

        /// <summary>
        /// Create a new DhcpException based on error code
        /// </summary>
        /// <param name="code">A DHCPSAPI or WIN32 error code</param>
        public DhcpException(uint code)
        {
            DhcpErrCode = code;
            if(!Enum.IsDefined(typeof(DhcpsapiErrorType), code))
            { isDhcpErr = false; }
        }

        /// <summary>
        /// Message string based on error code from constructor
        /// </summary>
        public override string Message
        {
            get
            {
                if (isDhcpErr)
                {
                    return DhcpsapiError.GetDesc((DhcpsapiErrorType)DhcpErrCode);
                }
                else
                {
                    Win32Exception ex = new Win32Exception((int)DhcpErrCode);
                    return ex.Message;
                }
                
            }
        }
    }
}

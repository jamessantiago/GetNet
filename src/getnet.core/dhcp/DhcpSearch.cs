
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dwo
{
    /// <summary>
    /// Defines a set of regular expression as search terms
    /// </summary>
    /// <remarks>Pattern strings are marked with IgnoreCase.  Please see <see cref="System.Text.RegularExpressions.Regex"/></remarks>
    /// <seealso href="http://msdn2.microsoft.com/en-us/library/az24scfc(VS.80).aspx">Regular Expression Language Elements</seealso>
    public class DhcpClientFilter
    {
        private Regex ipRegEx;
        /// <summary>
        /// IP regex pattern string
        /// </summary>
        public String IpRegEx
        {
            get { return ipRegEx.ToString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    ipRegEx = null;
                else
                    ipRegEx = new Regex(value, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            }
        }

        private Regex macRegEx;
        /// <summary>
        /// MAC regex pattern string
        /// </summary>
        public String MacRegEx
        {
            get { return macRegEx.ToString(); }
            set 
            {
                if (String.IsNullOrEmpty(value))
                    macRegEx = null;
                else
                    macRegEx = new Regex(value, RegexOptions.IgnoreCase | RegexOptions.Compiled);
 
            }
        }

        private Regex nameRegEx;
        /// <summary>
        /// Hostname regex pattern string
        /// </summary>
        public String NameRegEx
        {
            get { return nameRegEx.ToString(); }
            set 
            {
                if (String.IsNullOrEmpty(value))
                    nameRegEx = null;
                else
                    nameRegEx = new Regex(value, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// Dummy constructor
        /// </summary>
        public DhcpClientFilter() { }

        /// <summary>
        /// Filter predicate used by List&lt;DhcpClient&gt; Find and FindAll methods
        /// </summary>
        /// <param name="c">Client to match</param>
        /// <remarks>This filter matches based on a logical AND condition of non-null/non-empty regex members</remarks>
        /// <returns>True if match, otherwise false</returns>
        public bool Filter(DhcpClient c)
        {
            if (nameRegEx == null && ipRegEx == null && macRegEx == null)
                return false;

            if ((nameRegEx == null || nameRegEx.IsMatch(c.Name)) &&
                (ipRegEx == null || ipRegEx.IsMatch(c.IpAddress.ToString())) &&
                (macRegEx == null || macRegEx.IsMatch(c.MacAddress.ToString())))
                return true;
            return false;
        }
    }

}

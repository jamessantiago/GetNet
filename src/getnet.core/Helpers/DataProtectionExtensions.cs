using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

namespace getnet.core.Helpers
{
    public static class DataProtectionExtensions
    {
        public static string PersistentUnprotect(
            this IPersistedDataProtector dp,
            string protectedData,
            out bool requiresMigration,
            out bool wasRevoked)
        {
            try
            {
                bool ignoreRevocation = true;
                byte[] protectedBytes = Convert.FromBase64String(protectedData);
                byte[] unprotectedBytes = dp.DangerousUnprotect(protectedBytes, ignoreRevocation, out requiresMigration, out wasRevoked);

                return Encoding.UTF8.GetString(unprotectedBytes);
            } catch
            {
                requiresMigration = false;
                wasRevoked = false;
                return "";
            }
        }

        public static string PersistentProtect(
            this IPersistedDataProtector dp,
            string clearText)
        {
            try
            {
                byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
                byte[] protectedBytes = dp.Protect(clearBytes);
                string result = Convert.ToBase64String(protectedBytes);
                return result;
            } catch
            {
                return "";
            }

        }

    }
}

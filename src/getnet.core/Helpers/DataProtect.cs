using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using getnet.core.Helpers;

namespace getnet
{
    public class DataProtect
    {
        public DataProtect(IDataProtectionProvider dataProtectionProvider)
        {
            rawProtector = dataProtectionProvider.CreateProtector("getnet.core");
        }
        
        private IDataProtector rawProtector = null;
        private IPersistedDataProtector dataProtector
        {
            get { return rawProtector as IPersistedDataProtector; }
        }

        public string Protect(string data) => dataProtector.PersistentProtect(data);

        public string UnProtect(string data)
        {
            bool requiresMigration = false;
            bool wasRevoked = false;
            return dataProtector.PersistentUnprotect(data, out requiresMigration, out wasRevoked);
        }


    }
}

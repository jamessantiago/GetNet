using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using Novell.Directory.Ldap;

namespace getnet.Models.Security
{
    public class ActiveDirectoryProvider : SecurityProvider
    {

        public ActiveDirectoryProvider()
        {
        }

        public override bool InGroups(string groupNames, string accountName)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
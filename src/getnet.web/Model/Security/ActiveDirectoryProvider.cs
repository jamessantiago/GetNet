using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using Novell.Directory.Ldap;

namespace getnet.Model.Security
{
    public class ActiveDirectoryProvider : SecurityProvider
    {

        public ActiveDirectoryProvider()
        {
        }

        public override bool InGroups(string groupNames, string accountName)
        {
            foreach (var group in groupNames.Split(';'))
            {
                if (LdapServer.Current.InGroup(accountName, group.Trim()))
                    return true;
            }
            return false;
        }

        public override bool ValidateUser(string userName, string password)
        {
            return LdapServer.Current.Authenticate(userName, password);
        }
    }
}
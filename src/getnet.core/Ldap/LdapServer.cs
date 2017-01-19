using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace getnet.Model.Security
{
    public sealed class LdapServer
    {
        private static volatile LdapServer current;
        private static object syncRoot = new Object();
        private LdapConnection conn;
        private LdapSearchConstraints searchConstraints = new LdapSearchConstraints()
        {
            ReferralFollowing = true,
            BatchSize = 2000,
            HopLimit = 5,
            MaxResults = 2000,
            ServerTimeLimit = 10000,
            TimeLimit = 10000
        };

        private LdapServer() {
            var configExists = CoreCurrent.Configuration.GetSection("Security")?.GetSection("Ldap").GetChildren().Where(d =>
                d.Key == "Host" ||
                d.Key == "LoginDN" ||
                d.Key == "Password"
                ).Count() == 3;

            if (!configExists)
                throw new NotImplementedException("The ldap configuration is missing or incomplete");

            int ldapPort, ldapVersion;

            if (!int.TryParse(CoreCurrent.Configuration["Security:LdapPort"], out ldapPort))
                ldapPort = LdapConnection.DEFAULT_PORT;

            if (!int.TryParse(CoreCurrent.Configuration["Security:LdapVersion"], out ldapVersion))
                ldapVersion = LdapConnection.Ldap_V3;
            
            conn = new LdapConnection();
            conn.SecureSocketLayer = false;
            conn.Connect(CoreCurrent.Configuration["Security:Ldap:Host"], ldapPort);
            conn.Bind(ldapVersion, CoreCurrent.Configuration.GetSecure("Security:Ldap:LoginDN"), 
                CoreCurrent.Configuration.GetSecure("Security:Ldap:Password"));
        }

        public static LdapServer Current
        {
            get
            {
                if (current == null)
                    lock (syncRoot)
                        if (current == null)
                            current = new LdapServer();
                return current;
            }
        }

        public string BaseDN => conn.AuthenticationDN.Substring(conn.AuthenticationDN.IndexOf("DC="), conn.AuthenticationDN.Length - conn.AuthenticationDN.IndexOf("DC="));

        public bool Authenticate(string email, string password)
        {
            var dn = FindUserDN(email);
            var testconn = new LdapConnection();
            testconn.SecureSocketLayer = false;
            testconn.Connect(CoreCurrent.Configuration["Security:Ldap:Host"], conn.Port);

            testconn.Bind(conn.ProtocolVersion, dn, password);
            var connected = testconn.Bound;
            testconn.Disconnect();
            testconn.Dispose();
            return connected;
        }

        public bool InGroup(string email, string groupname)
        {
            if (groupname == "Domain Users")
            {
                var user = FindUser(email);
                return (user.getAttribute("primaryGroupID").StringValue == "513");
            }
            var userDN = FindUserDN(email);
            var results = conn.Search(BaseDN, LdapConnection.SCOPE_SUB,
                string.Format("(&(objectCategory=group)(sAMAccountName={0}))", groupname),
                null, false, searchConstraints);
            
            var entry = results.next();
            return entry.getAttribute("member").StringValueArray.Any(d => d.Equals(userDN, StringComparison.CurrentCultureIgnoreCase));
            
        }

        public string FindUserDN(string email)
        {
            return FindUser(email).DN;
        }

        public LdapEntry FindUser(string username)
        {
            var results = conn.Search(BaseDN, LdapConnection.SCOPE_SUB,
                string.Format("(&(objectCategory=person)(objectClass=user)(mail={0}))", username),
                null, false, searchConstraints);

            return results.next();
        }

        public void EnsureBind()
        {
            if (!conn.Bound)
                lock (syncRoot)
                    current = new LdapServer();
        }
    }
}

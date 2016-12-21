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
                ldapPort = LdapConnection.DEFAULT_SSL_PORT;

            if (!int.TryParse(CoreCurrent.Configuration["Security:LdapVersion"], out ldapVersion))
                ldapVersion = LdapConnection.Ldap_V3;
            
            conn = new LdapConnection();
            conn.SecureSocketLayer = true;
            conn.Connect(CoreCurrent.Configuration["Security:Ldap:Host"], ldapPort);
            
            conn.Bind(ldapVersion, CoreCurrent.Configuration.GetSecure("Security:Ldap:LoginDN"), CoreCurrent.Configuration.GetSecure("Security:Ldap:Password"));
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

        public bool Authenticate(string username, string password)
        {
            var dn = FindUserDN(username);
            var testconn = new LdapConnection();
            testconn.SecureSocketLayer = true;
            testconn.Connect(CoreCurrent.Configuration["Security:Ldap:Host"], conn.Port);

            testconn.Bind(conn.ProtocolVersion, dn, password);
            var connected = testconn.Bound;
            testconn.Disconnect();
            testconn.Dispose();
            return connected;
        }

        public bool InGroup(string username, string groupname)
        {
            var results = conn.Search("", LdapConnection.SCOPE_SUB,
                string.Format("(&(objectCategory=person)(objectClass=user)(sAMAccountName={0}))", groupname),
                new string[] { "memberof" }, false);

            try
            {
                var entry = results.next();
                return entry.getAttribute("memberof").StringValueArray.Any(d => d.Equals(groupname, StringComparison.CurrentCultureIgnoreCase));
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public string FindUserDN(string username)
        {
            var results = conn.Search("", LdapConnection.SCOPE_SUB,
                string.Format("(&(objectCategory=person)(objectClass=user)(sAMAccountName={0}))", username),
                null, false);

            try
            {
                var entry = results.next();
                return entry.DN;
            } catch (Exception ex)
            {
                throw ex; // :shrug:
            }

        }
    }
}

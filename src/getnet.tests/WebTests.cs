using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.Model.Security;
using Xunit;

namespace getnet.tests
{
    public class WebTests
    {
        [Fact]
        public void TestLdapConnection()
        {
            CoreCurrent.Configuration.Set("Security:Ldap:Roles:Test", "Test");
            //CoreCurrent.Configuration.Set("Security:Ldap:Host", "192.168.157.131");
            //CoreCurrent.Configuration.SetSecure("Security:Ldap:LoginDN", "CN=ldapuser,CN=Users,DC=getnet,DC=local");
            //CoreCurrent.Configuration.SetSecure("Security:Ldap:Password", "TestPassword123");
            //Assert.NotNull(LdapServer.Current);

            //Assert.True(LdapServer.Current.Authenticate("testuser", "TestPassword456"));
            //Assert.True(LdapServer.Current.InGroup("testuser", "Domain Users"));

        }
    }
}

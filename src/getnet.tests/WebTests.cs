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
            CoreCurrent.Configuration.SetSecure("Security:Ldap:LoginDN", "test");
            CoreCurrent.Configuration.SetSecure("Security:Ldap:Password", "test");
            var x = getnet.Model.Security.LdapServer.Current;
            Assert.NotNull(x);
        }
    }
}

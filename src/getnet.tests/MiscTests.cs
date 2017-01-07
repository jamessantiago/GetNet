using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using getnet.core.ssh;
using System.Net;

namespace getnet.tests
{
    public class MiscTests
    {
        [Fact]
        public void IpNetworkTest()
        {
            var net = IPNetwork.Parse("172.168.1.1/30");
            Assert.Equal(net.FirstUsable.ToString(), "172.168.1.1");
            Assert.Equal(net.LastUsable.ToString(), "172.168.1.2");
        }
    }
}

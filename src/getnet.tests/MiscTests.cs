using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using getnet.core.ssh;
using System.Net;
using System.IO;
using System.Threading;

namespace getnet.tests
{
    public class MiscTests
    {
        [Fact]
        public void IpNetworkTest()
        {
            var net = IPNetwork.Parse("172.168.1.1/24");
            Assert.Equal(net.FirstUsable.ToString(), "172.168.1.1");
            Assert.Equal(net.LastUsable.ToString(), "172.168.1.2");
        }

        [Fact]
        public void DirectoryTest()
        {
            Directory.SetCurrentDirectory(@"D:\Code\getnet");
        }

        [Fact]
        public void DnsTest()
        {
            var cts = new CancellationTokenSource();
            IPHostEntry hostname = null;
            cts.CancelAfter(TimeSpan.FromTicks(1));
            hostname = Task.Run(() => Dns.GetHostEntryAsync("172.16.100.10").Result, cts.Token).Result;
            Assert.Equal("something", hostname.HostName);
        }
    }
}

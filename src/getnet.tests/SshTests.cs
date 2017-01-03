using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using getnet.core.ssh;

namespace getnet.tests
{
    public class SshTests
    {
        [Fact]
        public void ConnectSshTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.32.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            try
            {
                var rawdata = client.Execute<RawSshData>("show cats");
                Assert.NotNull(rawdata.First().Data);
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }

        [Fact]
        public void CdpNeighborTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.32.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var neighbors = client.Execute<CdpNeighbor>();
            Assert.Equal(neighbors.Count, 2);
        }

        [Fact]
        public void TunnelInterfaceTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.32.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<TunnelInterface>();
            Assert.Equal(ints.Count, 1);
        }

        [Fact]
        public void InventoryTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.32.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<Inventory>();
            Assert.Equal(ints.Count, 6);
        }

        [Fact]
        public void VersionTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.32.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<DeviceVersion>();
            Assert.Equal(ints.Count, 1);
        }

        [Fact]
        public void VlanTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "172.16.100.241",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<Vlan>();
            Assert.Equal(ints.Count, 1);
        }
    }
}

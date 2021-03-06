﻿using System;
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
            Assert.Equal(neighbors.Count, 1);
            Assert.True(!neighbors.First().Hostname.Contains("test.local"));
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

        [Fact]
        public void IpIntTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.33.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<IpInterface>().Where(d => d.Interface == "x").ToList();
            Assert.Equal(ints.Count, 1);
        }

        [Fact]
        public void ArpTest()
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.33.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<Arp>();
            Assert.Equal(ints.Count, 4);
        }

        [Fact]
        public void MacTest()
        {
            CoreCurrent.Configuration["ASPNETCORE_ENVIRONMENT"] = "Production";
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "172.16.100.241",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<MacAddress>("show mac-add");
            Assert.Equal(ints.Count, 3);
        }

        [Fact]
        public void IntDesTest()
        {
            CoreCurrent.Configuration["ASPNETCORE_ENVIRONMENT"] = "Development";
            IGsc client = new RenciSshClient(new RenciSshClientSettings()
            {
                Host = "192.168.32.1",
                Username = "admin",
                Password = "password",
                Port = 22
            });

            var ints = client.Execute<InterfaceDescription>();
            Assert.Equal(ints.Count, 4);
        }
    }
}

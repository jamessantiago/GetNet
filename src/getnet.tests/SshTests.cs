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
                var rawdata = (RawSshData)client.Execute<RawSshData>("show cats");
                Assert.NotNull(rawdata.Data);
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }
    }
}

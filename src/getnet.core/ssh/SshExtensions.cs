using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public static class SshExtensions
    {
        public static IGsc Ssh(this string ip)
        {
            IGsc client = new RenciSshClient(new RenciSshClientSettings
            {
                Host = ip,
                Username = CoreCurrent.Configuration.GetSecure("SSH:Username"),
                Password = CoreCurrent.Configuration.GetSecure("SSH:Password"),
                Port = 22
            });
            return client;
        }
    }
}

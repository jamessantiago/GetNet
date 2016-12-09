using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace getnet.core.ssh
{
    public interface IgetnetSshClientFactory
    {
        SshClient CreateClient(IgetnetSshClientSettings settings);
    }
}

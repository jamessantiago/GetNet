using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public interface IgetnetSshClientSettings
    {
        string Host { get; set; }
        int Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string KeyFile { get; set; }
        string Passphrase { get; set; }
    }
}

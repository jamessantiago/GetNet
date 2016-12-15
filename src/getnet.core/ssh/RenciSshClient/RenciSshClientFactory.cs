using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace getnet.core.ssh
{
    public class RenciSshClientFactory : IGscFactory
    {
        public SshClient CreateClient(IGscSettings settings)
        {
            ConnectionInfo con = new ConnectionInfo(settings.Host, settings.Username,
                new AuthenticationMethod[] {
                    new PasswordAuthenticationMethod(settings.Username, settings.Password)
                });
            if (settings.KeyFile.HasValue() && settings.Password.HasValue())
                con.AuthenticationMethods.Add(
                    new PrivateKeyAuthenticationMethod(settings.Username, new PrivateKeyFile[]
                    {
                        new PrivateKeyFile(settings.KeyFile, settings.Passphrase)
                    }));
            var client = new SshClient(con); 
            client.Connect();            
            return client;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using getnet.core.Model.Entities;

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

        public static IGsc Ssh(this IPAddress ip)
        {
            return ip.ToString().Ssh();
        }

        public static bool CanSsh(this string ip)
        {
            return IPAddress.Parse(ip).CanSsh();
        }

        public static bool CanSsh(this IPAddress ip)
        {
            try
            {
                using (var tc = new System.Net.Sockets.TcpClient())
                {
                    tc.ReceiveTimeout = 200;
                    tc.SendTimeout = 200;
                    tc.Client.Connect(ip, 22);
                    return true;
                }
            } catch
            {
                return false;
            }
        }

        public static NetworkCapabilities GetCaps(this string[] capabilities)
        {
            NetworkCapabilities caps = 0;
            foreach (var cap in capabilities)
            {
                switch (cap)
                {
                    case "Router":
                        caps = caps | NetworkCapabilities.Router;
                        break;
                    case "Switch":
                        caps = caps | NetworkCapabilities.Switch;
                        break;
                    default:
                        break;
                }
            }
            return caps;
        }
    }
}

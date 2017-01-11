using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using getnet;
using System.Net;

namespace getnet.core.ssh
{
    public class IpInterface : ICommandResult
    {
        public string Interface { get; set; }
        public IPAddress IP { get; set; }
        public IPNetwork IPNetwork { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var matchSet1 = Regex.Matches(data, @"([\w\/]+)([\w ,]*)[\n|\r|\r\n]\s*Internet address is ([\d\.]*)\/(\d\d)", RegexOptions.Multiline);

            foreach (Match m in matchSet1)
            {
                if (!(string.IsNullOrEmpty(m.Groups[1].Value) || string.IsNullOrEmpty(m.Groups[3].Value) || string.IsNullOrEmpty(m.Groups[4].Value)))
                {
                    string port = m.Groups[1].Value;
                    string network = m.Groups[3].Value + "/" + m.Groups[4].Value;
                    results.Add(new IpInterface
                    {
                        Interface = port,
                        IP = IPAddress.Parse(m.Groups[3].Value),
                        IPNetwork = IPNetwork.Parse(network)
                    });
                }
            }

            return results;
        }

        public string GetStoredCommand()
        {
            return "show ip interface";
        }
    }
}

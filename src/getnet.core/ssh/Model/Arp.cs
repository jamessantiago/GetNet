using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using getnet;
using System.Net;

namespace getnet.core.ssh
{
    public class Arp : ICommandResult
    {
        public IPAddress IP { get; set; }
        public string Mac { get; set; }
        public string Interface { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var ipMatchSet = Regex.Matches(data, @"(\d+\.\d+\.\d+\.\d+) \s*");
            var macIntMatchSet = Regex.Matches(data, @"(\w{4}\.\w{4}\.\w{4}|INCOMPLETE)[\w\s,]*([VFGST][\w\/]{2,30})");

            if (Util.AllSame(ipMatchSet.Count, macIntMatchSet.Count))
            {
                for (int i = 0; i < ipMatchSet.Count; i++)
                {
                    if (macIntMatchSet[i].Groups[1].Value == "INCOMPLETE") continue;
                    results.Add(new Arp
                    {
                        IP = IPAddress.Parse(ipMatchSet[i].Groups[1].Value),
                        Mac = macIntMatchSet[i].Groups[1].Value,
                        Interface = macIntMatchSet[i].Groups[2].Value
                    });
                }
            }

            return results;
        }

        public string GetStoredCommand()
        {
            return "show ip arp";
        }
    }
}

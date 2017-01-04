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
            var ipMatchSet = Regex.Matches(data, @"((([2]([0-4][0-9]|[5][0-5])|[0-1]?[0-9]?[0-9])[.]){3}(([2]([0-4][0-9]|[5][0-5])|[0-1]?[0-9]?[0-9])))\s*[-\d]*\s\s\s(\w{4}\.\w{4}\.\w{4})");
            var macIntMatchSet = Regex.Matches(data, @"(\w{4}\.\w{4}\.\w{4})[\w\s,]*([FGST][\w\/]{2,30})");

            if (Util.AllSame(ipMatchSet.Count, macIntMatchSet.Count))
            {
                for (int i = 0; i < ipMatchSet.Count; i++)
                {
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
            return "show arp";
        }
    }
}

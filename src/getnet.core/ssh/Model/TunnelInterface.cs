using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace getnet.core.ssh
{
    public class TunnelInterface : ICommandResult
    {
        public TunnelInterface() { }

        public string Interface { get; set; }
        public IPAddress DestinationIp { get; set; }
        public string Protocol { get; set; }
        public string MTU { get; set; }


        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var ints = Regex.Matches(data, @"(Tu(nnel)?\d+) is");
            var destIps = Regex.Matches(data, @"destination ([\d\.]*)");
            var prots = Regex.Matches(data, @"protocol/transport ([\w\d/\.]*)");
            var mtus = Regex.Matches(data, @"MTU (\d*)");

            if (!Util.AllSame(ints.Count, destIps.Count, prots.Count, mtus.Count))
                throw new Exception("Parsing failed to parse properties in an equal ammount from the interface table");

            for (int i = 0; i < ints.Count; i++)
            {
                results.Add(new TunnelInterface()
                {
                    Interface = ints[i].Groups[1].Value,
                    DestinationIp = IPAddress.Parse(destIps[i].Groups[1].Value),
                    Protocol = prots[i].Groups[1].Value,
                    MTU = mtus[i].Groups[1].Value
                });
            }

            return results;
        }

        public string GetStoredCommand()
        {
            return "show interfaces | incl Tu";
        }
    }
}

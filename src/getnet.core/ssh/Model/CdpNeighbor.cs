using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using getnet;
using System.Net;

namespace getnet.core.ssh
{
    public class CdpNeighbor : ICommandResult
    {
        public CdpNeighbor()
        {

        }

        public CdpNeighbor(IPAddress ip, string hostname, string model, string inPort, string outPort)
        {
            IP = ip;
            Hostname = hostname;
            Model = model;
            InPort = inPort;
            OutPort = outPort;
        }

        public IPAddress IP { get; private set; }
        public string Hostname { get; private set; }
        public string Model { get; private set; }
        public string InPort { get; private set; }
        public string OutPort { get; private set; }
        
        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var IPs = Regex.Matches(data, @"Entry address\(es\):\s*IP address: ([\d\.]*)", RegexOptions.Multiline);
            var hostnames = Regex.Matches(data, @"Device ID: ([\w\.-]*)");
            var models = Regex.Matches(data, @"Platform: ([\w\. -]*)");
            var inPorts = Regex.Matches(data, @"Interface: ([\w\/]*)");
            var outPorts = Regex.Matches(data, @"\(outgoing port\): ([\w\/]*)");

            if (!AllSame(IPs.Count, hostnames.Count, models.Count, inPorts.Count, outPorts.Count))
                throw new Exception("Parsing failed to parse properties in an equal ammount from the CDP table");
            
            for (int i = 0; i < IPs.Count; i++)
            {
                results.Add(new CdpNeighbor(
                    ip: IPAddress.Parse(IPs[i].Groups[1].Value),
                    hostname: hostnames[i].Groups[1].Value,
                    model: models[i].Groups[1].Value,
                    inPort: inPorts[i].Groups[1].Value,
                    outPort: outPorts[i].Groups[1].Value));
            }
            
            return results;
        }

        private static bool AllSame(params int[] list)
        {
            return (list as IEnumerable<int>).AllSame();
        }

        public string GetStoredCommand()
        {
            return "show cdp neighbors detail";
        }
    }
}

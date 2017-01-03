﻿using System;
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

        public IPAddress IP { get; set; }
        public string Hostname { get; set; }
        public string Model { get; set; }
        public string InPort { get; set; }
        public string OutPort { get; set; }
        public string[] Capabilities { get; set; }
        
        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var IPs = Regex.Matches(data, @"Entry address\(es\):\s*IP address: ([\d\.]*)", RegexOptions.Multiline);
            var hostnames = Regex.Matches(data, @"Device ID: ([\w\.-]*)");
            var models = Regex.Matches(data, @"Platform: ([\w\. -]*)");
            var inPorts = Regex.Matches(data, @"Interface: ([\w\/]*)");
            var outPorts = Regex.Matches(data, @"\(outgoing port\): ([\w\/]*)");
            var caps = Regex.Matches(data, @"Capabilities: ([\w, ]*)");

            if (!Util.AllSame(IPs.Count, hostnames.Count, models.Count, inPorts.Count, outPorts.Count))
                throw new Exception("Parsing failed to parse properties in an equal ammount from the CDP table");

            for (int i = 0; i < IPs.Count; i++)
            {
                results.Add(new CdpNeighbor() {
                    IP = IPAddress.Parse(IPs[i].Groups[1].Value),
                    Hostname = hostnames[i].Groups[1].Value,
                    Model = models[i].Groups[1].Value,
                    InPort = inPorts[i].Groups[1].Value,
                    OutPort = outPorts[i].Groups[1].Value,
                    Capabilities = caps[i].Groups[1].Value.Split(", ".ToCharArray()).Select(d => d.Trim()).ToArray()
                });
            }
            
            return results;
        }

        public string GetStoredCommand()
        {
            return "show cdp neighbors detail";
        }
    }
}

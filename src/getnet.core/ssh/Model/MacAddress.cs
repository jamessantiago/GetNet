using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using getnet;
using System.Net;

namespace getnet.core.ssh
{
    public class MacAddress : ICommandResult
    {
        public string Mac { get; set; }
        public string Interface { get; set; }
        public IPAddress TableOwner { get; set; }
        public int Level { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var matchSet = Regex.Matches(data, @"(\w{4}\.\w{4}\.\w{4})[\w\s,]*([PLVFGST][\w\/]{2,30})");
            for(int i = 0; i < matchSet.Count; i++ )
            {
                results.Add(new MacAddress
                {
                    Mac = matchSet[i].Groups[1].Value,
                    Interface = matchSet[i].Groups[2].Value
                });
            }

            return results;
        }

        public string GetStoredCommand()
        {
            return "show mac address-table";  //mac-address-table for routers...
        }
    }
}

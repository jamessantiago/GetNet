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

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            if (CoreCurrent.Configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                data = @"
Destination Address  Address Type  VLAN  Destination Port
-------------------  ------------  ----  --------------------
c007.07e2.0000          Self          1     Vlan1
ca03.13b7.001c          Dynamic       1     FastEthernet1/0
0050.7966.6801          Dynamic       1     FastEthernet1/1
";
            }
            var results = new List<ICommandResult>();
            var matchSet = Regex.Matches(data, @"(\w{4}\.\w{4}\.\w{4})[\w\s,]*([LVFGST][\w\/]{2,30})");
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
            return "show mac-address-table dynamic";
        }
    }
}

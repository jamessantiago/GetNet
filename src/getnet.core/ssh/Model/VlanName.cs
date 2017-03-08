using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public class VlanName : ICommandResult
    {
        public int VlanNumber { get; set; }
        public string Name { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var matchSet = Regex.Matches(data, @"(\d{1,3})\s+([\w\d_\-]+)");
            foreach (Match match in matchSet)
            {
                results.Add(new VlanName
                {
                    VlanNumber = int.Parse(match.Groups[1].Value),
                    Name =  match.Groups[2].Value
                });
            }
            return results;
        }

        public string GetStoredCommand()
        {
            return "show vlan";
        }
    }
}

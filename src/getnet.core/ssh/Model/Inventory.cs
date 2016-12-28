using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace getnet.core.ssh
{
    public class Inventory : ICommandResult
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PID { get; set; }
        public string Serial { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var names = Regex.Matches(data, @"NAME: ""([\w\d \(\)/\-,]*)""");
            var descs = Regex.Matches(data, @"DESCR: ""([\w\d \(\)/\-,]*)""");
            var pids = Regex.Matches(data, @"PID: ([\w\d\(\)/\-,]*)");
            var sns = Regex.Matches(data, @"SN: ([\w\d]+)?");

            if (!Util.AllSame(names.Count, descs.Count, pids.Count, sns.Count))
                throw new Exception("Parsing failed to parse properties in an equal ammount from the inventory table");

            for (int i = 0; i < names.Count; i++)
            {
                results.Add(new Inventory()
                {
                    Name = names[i].Groups[1].Value,
                    Description = names[i].Groups[1].Value,
                    PID = pids[i].Groups[1].Value,
                    Serial = sns[i].Groups[1].Value
                });
            }

            return results;
        }

        public string GetStoredCommand()
        {
            return "show inventory";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace getnet.core.ssh
{
    public class InterfaceDescription : ICommandResult
    {
        public string Interface { get; set; }
        public string Status { get; set; }
        public string Protocol { get; set; }
        public string Description { get; set; }

        public bool IsUp => Status.Equals("up", StringComparison.CurrentCultureIgnoreCase) && Protocol.Equals("up", StringComparison.CurrentCultureIgnoreCase);

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            foreach (Match match in Regex.Matches(data, @"([FGSTV]\w+?\d\/?\d?) +(\w+\s?\w+?) +(\w+\s?\w+?) +(.*)"))
            {
                results.Add(new InterfaceDescription
                {
                    Interface = match.Groups[1].Value,
                    Status = match.Groups[2].Value,
                    Protocol = match.Groups[3].Value,
                    Description = match.Groups[4].Value.Trim()
                });
            }            

            return results;
        }

        public string GetStoredCommand()
        {
            return "show interface description";
        }
    }
}

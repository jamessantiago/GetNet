using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using getnet;
using System.Net;

namespace getnet.core.ssh
{
    public class Vlan : ICommandResult
    {
        public int VlanNumber { get; set; }
        public IPNetwork IPNetwork { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var matchSet1 = Regex.Matches(data, @"[0-9]\.([0-9]{1,3})((?!Vlan)[\w ,]*)[\n|\r|\r\n]\s*Internet address is ([\d\.]*)\/(\d\d)", RegexOptions.Multiline);
            var matchSet2 = Regex.Matches(data, @"Vlan([0-9]{1,3})((?!Vlan)[\w ,]*)[\n|\r|\r\n]{1,5}\s*Internet address is ([\d\.]*)\/(\d\d)", RegexOptions.Multiline);
            var matchSet3 = Regex.Matches(data, @"Vlan([0-9]{1,3})((?!Vlan)[\w ,]*).{1,800}Secondary address ([\d\.]*)\/(\d\d)", RegexOptions.Singleline);

            IEnumerable<Match> fullMatchSet = matchSet1.OfType<Match>().Concat(matchSet2.OfType<Match>()).Concat(matchSet3.OfType<Match>()).Where(m => m.Success);

            foreach (Match m in fullMatchSet)
            {
                if (!(string.IsNullOrEmpty(m.Groups[1].Value) || string.IsNullOrEmpty(m.Groups[2].Value) || string.IsNullOrEmpty(m.Groups[3].Value)))
                {
                    string vlannumber = m.Groups[1].Value;
                    string network = m.Groups[3].Value + "/" + m.Groups[4].Value;
                    results.Add(new Vlan
                    {
                        VlanNumber = int.Parse(vlannumber),
                        IPNetwork = IPNetwork.Parse(network)
                    });
                }
            }

            return results;
        }

        public string GetStoredCommand()
        {
            return "show ip interface";
        }
    }
}

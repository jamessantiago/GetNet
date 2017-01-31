using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace getnet.core.ssh
{
    public class DeviceVersion : ICommandResult
    {
        public string Hostname { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }

        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var ver = new DeviceVersion();
            ver.Hostname = Regex.Match(data, @"(.*) uptime").Groups[1].Value;
            MatchCollection models = Regex.Matches(data, @"(.*) processor (\(revision|with)");
            if (models.Count == 0)
                models = Regex.Matches(data, @"(.*) \(revision");
            if (models.Count > 0)
                ver.Model = models[0].Groups[1].Value;
            ver.Serial = Regex.Match(data, @"Processor board ID\s([\w]*)").Groups[1].Value;

            if (!ver.Hostname.HasValue() || !ver.Model.HasValue() || !ver.Serial.HasValue())
                throw new Exception("Incomplete data returned from device");

            results.Add(ver);

            return results;
        }

        public string GetStoredCommand()
        {
            return "show version";
        }

    }    
}

using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace getnet.core.Helpers
{
    public class ConfigurationSerializer
    {
        public Dictionary<string, string> Serialize(object o)
        {
            var serialized =
                JsonConvert.SerializeObject(
                    o,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized)))
            {
                var jsonProvider = new CustomJsonConfigurationProvider();

                return jsonProvider
                    .GetData(ms)
                    .ToDictionary(key => key.Key, value => value.Value);
            }
        }
    }

    public class CustomJsonConfigurationProvider : JsonConfigurationProvider
    {
        public CustomJsonConfigurationProvider() : base(new JsonConfigurationSource())
        {
        }

        public IDictionary<string, string> GetData(Stream s)
        {
            Load(s);
            return Data;
        }
        
        public override void Set(string key, string value)
        {
            value = DPAPI.Encrypt(DPAPI.KeyType.MachineKey, value, CoreCurrent.ENTROPY);
            base.Set(key, value);
            var sb = new StringBuilder();
            var header = "Encrypted with machine key on " + System.DateTime.UtcNow.ToString();
            header = header.PadLeft((80+header.Length)/2, '-').PadRight(80, '-');
            sb.AppendLine(header);
            sb.AppendLine(DPAPI.Encrypt(DPAPI.KeyType.MachineKey, JsonConvert.SerializeObject(this.Data), CoreCurrent.ENTROPY));
            var footer = "End of encrypted file";
            footer = footer.PadLeft((80 + footer.Length) / 2, '-').PadRight(80, '-');
            sb.AppendLine(footer);
            System.IO.File.WriteAllText(this.Source.Path, sb.ToString());
        }

        public override bool TryGet(string key, out string value)
        {
            if (!base.TryGet(key, out value) || !value.HasValue())
                return false;
            else
            {
                try
                {
                    string descript = string.Empty;
                    value = DPAPI.Decrypt(value, CoreCurrent.ENTROPY, out descript);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public class RawSshData : ICommandResult
    {
        public RawSshData()
        {
        }

        public string Data { get; private set; }
        
        public List<ICommandResult> ConvertCommandResult<T>(string data)
        {
            var results = new List<ICommandResult>();
            var result = new RawSshData();
            result.Data = data;
            results.Add(result);
            return results;
        }

        public string GetStoredCommand()
        {
            throw new NotImplementedException();
        }
    }
}

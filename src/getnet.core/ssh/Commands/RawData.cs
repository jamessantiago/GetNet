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
        
        public ICommandResult ConvertCommandResult<T>(string data)
        {
            var result = new RawSshData();
            result.Data = data;
            return result;
        }

        public string GetStoredCommand()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public interface ICommandResult
    {
        T ConvertCommandResult<T>(string data, int exitStatus);
    }
}

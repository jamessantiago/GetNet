using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public interface IGsc
    {
        ICommandResult Execute<T>(string command) where T : ICommandResult, new();
    }
}

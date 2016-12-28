using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public interface IGsc
    {
        List<T> Execute<T>() where T : ICommandResult, new();
        List<T> Execute<T>(string command) where T : ICommandResult, new();
    }
}

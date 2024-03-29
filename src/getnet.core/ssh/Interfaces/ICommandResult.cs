﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace getnet.core.ssh
{
    public interface ICommandResult
    {
        string GetStoredCommand();        
        List<ICommandResult> ConvertCommandResult<T>(string data);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace getnet.core.ssh
{
    public interface IGscFactory
    {
        SshClient CreateClient(IGscSettings settings);
    }
}

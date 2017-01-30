using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using DasMulli.Win32.ServiceUtils;

namespace getnet.service
{
    public class Service : IWin32Service
    {
        public string ServiceName => "GetNet Service";
        private Whistler logger = new Whistler();

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Task.Run(() => {
                Runner.Run();
            });
        }

        public void Stop()
        {
            Runner.ResetEvent.Set();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace getnet
{
    public class Program
    {
    public static void Main(string[] args)
    {
        var host = new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();
            
        CoreCurrent.Protector = ActivatorUtilities.CreateInstance<DataProtect>(host.Services);

        Current.Services = host.Services;

        Current.SetDbConfigurationState();

        host.Run(Current.AppCancellationSource.Token);
        Current.AppCancellationSource = new System.Threading.CancellationTokenSource();
        Main(args);
    }
    }
}

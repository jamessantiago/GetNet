using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System.Text;

namespace getnet
{
    public static partial class CoreCurrent
    {
        private static IConfigurationRoot configuration;

        public static IConfigurationRoot Configuration => configuration != null ? configuration : configuration = LoadConfiguration();

        private static IConfigurationRoot LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCustomJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            return builder.Build();
        }

        public const string ENTROPY = "getnet";
    }

    
}

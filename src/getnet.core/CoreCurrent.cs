using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Primitives;

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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            ConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            return builder.Build();
        }

        public static string ConfigFile { get; private set; }

        public const string ENTROPY = "getnet";
    }
}

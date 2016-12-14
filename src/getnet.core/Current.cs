using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace getnet
{
    public static partial class Current
    {
        private static IConfigurationRoot configuration;

        public static IConfigurationRoot Configuration => configuration != null ? configuration : configuration = LoadConfiguration();

        private static IConfigurationRoot LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            return builder.Build();
        }
    }
}

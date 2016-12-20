using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using getnet.core.Helpers;
using System.IO;

namespace Microsoft.Extensions.Configuration
{
    public static class CustomJsonConfigurationExtensions
    {
        public static IConfigurationBuilder AddCustomJsonFile(this IConfigurationBuilder builder, string path)
        {
            return AddCustomJsonFile(builder, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddCustomJsonFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddCustomJsonFile(builder, path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddCustomJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            var provider = new CustomJsonConfigurationProvider();
            provider.Source.Path = path;
            provider.Source.Optional = optional;
            provider.Source.ReloadOnChange = reloadOnChange;
            builder.Add(provider.Source);
            return builder;
        }
    }
}
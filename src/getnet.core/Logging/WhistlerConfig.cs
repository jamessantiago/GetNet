using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;
using getnet.core.Logging;

namespace getnet
{
    public static class WhistlerConfig
    {
        public static bool IsConfigured { get; private set; }

        public static void Configure()
        {
            if (IsConfigured)
                return;
            
            var config = new LoggingConfiguration();
            
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            var emailTarget = new MailKitTarget();
            config.AddTarget("email", emailTarget);

            if (!CoreCurrent.Configuration["Whistler:Console:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Console:Enabled", "false");
            if (!CoreCurrent.Configuration["Whistler:Console:Layout"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Console:Layout", @"${date:format=HH\:mm\:ss} ${logger} ${message}");
            if (!CoreCurrent.Configuration["Whistler:File:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:File:Enabled", "true");
            if (!CoreCurrent.Configuration["Whistler:File:FileName"].HasValue()) CoreCurrent.Configuration.Set("Whistler:File:FileName", "${basedir}/Logs/${shortdate}_getnet.log");
            if (!CoreCurrent.Configuration["Whistler:File:Layout"].HasValue()) CoreCurrent.Configuration.Set("Whistler:File:Layout", "Date: ${longdate};    Type: ${event-context:item=type};    Logger: ${logger};    Message: ${message};    Details: ${event-context:item=details};");
            if (!CoreCurrent.Configuration["Whistler:Smtp:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Smtp:Enabled", "false");


            consoleTarget.Layout = CoreCurrent.Configuration["Whistler:Console:Layout"];
            fileTarget.FileName = CoreCurrent.Configuration["Whistler:File:FileName"];
            fileTarget.Layout = CoreCurrent.Configuration["Whistler:File:Layout"];

            if (CoreCurrent.Configuration["Whistler:Console:Enabled"] == "true")
            {
                var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
                config.LoggingRules.Add(rule1);
            }

            if (CoreCurrent.Configuration["Whistler:File:Enabled"] == "true")
            {
                var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(rule2);
            }

            if (CoreCurrent.Configuration["Whistler:Smtp:Enabled"] == "true")
            {
                var rule3 = new LoggingRule("*", LogLevel.Debug, emailTarget);
                config.LoggingRules.Add(rule3);
            }
            
            LogManager.Configuration = config;
        }
    }
}

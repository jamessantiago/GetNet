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
            
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/Logs/${shortdate}_getnet.log";
            fileTarget.Layout = "Date: ${longdate};    Type: ${event-context:item=type};    Logger: ${logger};    Message: ${message};    Details: ${event-context:item=details};";
            
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            var rule3 = new LoggingRule("*", LogLevel.Debug, emailTarget);
            config.LoggingRules.Add(rule3);
            
            LogManager.Configuration = config;
        }
    }
}

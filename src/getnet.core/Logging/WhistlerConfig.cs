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

            var dbTarget = new DatabaseTarget();
            config.AddTarget("db", dbTarget);

            if (!CoreCurrent.Configuration["Whistler:Console:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Console:Enabled", "false");
            if (!CoreCurrent.Configuration["Whistler:Console:Layout"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Console:Layout", @"${date:format=HH\:mm\:ss} ${logger} ${message}");
            if (!CoreCurrent.Configuration["Whistler:File:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:File:Enabled", "true");
            if (!CoreCurrent.Configuration["Whistler:File:FileName"].HasValue()) CoreCurrent.Configuration.Set("Whistler:File:FileName", "${basedir}/Logs/${shortdate}_getnet.log");
            if (!CoreCurrent.Configuration["Whistler:File:Layout"].HasValue()) CoreCurrent.Configuration.Set("Whistler:File:Layout", "Date: ${longdate};    Type: ${event-context:item=type};    Logger: ${logger};    Message: ${message};    Details: ${event-context:item=details};");
            if (!CoreCurrent.Configuration["Whistler:Smtp:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Smtp:Enabled", "false");
            if (!CoreCurrent.Configuration["Whistler:Db:Enabled"].HasValue()) CoreCurrent.Configuration.Set("Whistler:Db:Enabled", "false");

            consoleTarget.Layout = CoreCurrent.Configuration["Whistler:Console:Layout"];
            fileTarget.FileName = CoreCurrent.Configuration["Whistler:File:FileName"];
            fileTarget.Layout = CoreCurrent.Configuration["Whistler:File:Layout"];
            if (CoreCurrent.Configuration["Data:DataStore"] == "MSSQL" && CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString").HasValue())
            {
                dbTarget.ConnectionString = CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString");
            }
            else if (CoreCurrent.Configuration["Data:DataStore"] == "Postgres" && CoreCurrent.Configuration.GetSecure("Data:NpgsqlConnectionString").HasValue())
            {
                dbTarget.ConnectionString = CoreCurrent.Configuration.GetSecure("Data:NpgsqlConnectionString");
            }
            dbTarget.CommandText = "INSERT INTO Events (TimeStamp, Host, Type, Source, Message, Level, Logger, Details, SiteId) VALUES(@date, @host, @type, @source, @message, @level, @logger, @details, @siteid)";
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@date", new NLog.Layouts.SimpleLayout("${date:universalTime=true}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@host", new NLog.Layouts.SimpleLayout("${machinename}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@type", new NLog.Layouts.SimpleLayout("${event-context:item=type}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@source", new NLog.Layouts.SimpleLayout("")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@message", new NLog.Layouts.SimpleLayout("${message}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@level", new NLog.Layouts.SimpleLayout("${level}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@logger", new NLog.Layouts.SimpleLayout("${logger}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@details", new NLog.Layouts.SimpleLayout("${event-context:item=details}")));
            dbTarget.Parameters.Add(new DatabaseParameterInfo("@siteid", new NLog.Layouts.SimpleLayout("${event-context:item=siteid}")));
            dbTarget.DBProvider = CoreCurrent.Configuration["Data:DataStore"] ?? "MSSQL";

            if (CoreCurrent.Configuration["Whistler:Console:Enabled"] == "true")
            {
                var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
                config.LoggingRules.Add(rule1);
            }

            if (CoreCurrent.Configuration["Whistler:File:Enabled"] == "true")
            {
                var rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
                config.LoggingRules.Add(rule2);
            }

            if (CoreCurrent.Configuration["Whistler:Smtp:Enabled"] == "true")
            {
                var rule3 = new LoggingRule("*", LogLevel.Debug, emailTarget);
                config.LoggingRules.Add(rule3);
            }

            if (CoreCurrent.Configuration["Whistler:Db:Enabled"] == "true")
            {
                var rule4 = new LoggingRule("*", LogLevel.Info, dbTarget);
                config.LoggingRules.Add(rule4);
            }
            
            LogManager.Configuration = config;
        }
    }
}

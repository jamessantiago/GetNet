using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using DasMulli.Win32.ServiceUtils;

namespace getnet.service
{
    public class Program
    {
        private static Whistler logger = new Whistler(typeof(Program).FullName);

        public static void Main(string[] args)
        {
            var service = false;
            var help = false;
            var conf = false;
            var windows = false;
            var setdir = "";
            var syn = ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("c|configure", ref conf, "Configure the GetNet service");
                syntax.DefineOption("s|service", ref service, "Run as a service");
                syntax.DefineOption("w|windows-service", ref windows, "Run as a windows service");
                syntax.DefineOption("d|working-directory", ref setdir, "Set the working directory");
                syntax.DefineOption("h|help", ref help, "Show help");
            });
            if (help) {
                Console.WriteLine(HelpTextGenerator.Generate(syn, 80));
                Environment.Exit(0);
            }

            if (setdir.HasValue())
            {
                setdir = setdir.Trim('\'', '"');
                Directory.SetCurrentDirectory(setdir);
                logger.Info("Current directory set to " + Directory.GetCurrentDirectory(), WhistlerTypes.ServiceControl);
            }

            IServiceCollection serviceCollection = new ServiceCollection();
            string pathToCryptoKeys = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                    + "dp_keys" + Path.DirectorySeparatorChar;
            serviceCollection.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(pathToCryptoKeys))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(3650))
                .SetApplicationName("getnet");
            CoreCurrent.Protector = ActivatorUtilities.CreateInstance<DataProtect>(serviceCollection.BuildServiceProvider());

            CoreCurrent.Configuration["Whistler:Console:Enabled"] = "true";

            if (conf)
            {
                Configuration.StartConfig();
            }

            if (!Configuration.VerifyConfig())
            {
                logger.Error("Configuration is missing.  Setup GetNet with the -c switch and specify the configuration", WhistlerTypes.ServiceControl);
                Environment.Exit(1);
            }

            if (service)
            {
                Console.WriteLine(LOGO);
                logger.Info("GetNet Service is starting up", WhistlerTypes.ServiceControl);
                Runner.Run();
            }
            else if (windows)
            {
                logger.Info("Running as windows service", WhistlerTypes.ServiceControl);
                var thisService = new Service();
                var serviceHost = new Win32ServiceHost(thisService);
                serviceHost.Run();
            } else if (!conf)
            {
                logger.Error("Configuration or service option must be specified", WhistlerTypes.ServiceControl);
                Console.WriteLine(HelpTextGenerator.Generate(syn, 80));
                Environment.Exit(1);
            }
        }

        private const string LOGO = @"

                           GETNET SERVICE

                     -o/.             -/.`                  
                     Gooo/.`          -ooo/`                
                     Eoooooo/`        -ooooo/-`             
                     Toooooooo+:`     -oooooooo/`           
                     Nooooooooooo:/   -oooooooooo.          
                     Eoooooooooooooo:`-oooooooooo.          
                     Toooooooooooooooo+oooooooooo.          
                     `:+ooooooooooooooooooooooooo.          
                        -+ooooooooooooooooooooooo.          
                           -+oooooooooooooooooooo.          
                            `:++ooooooooooooooooo.          
                               `-oooooooooooooooo.          
                          --      `:ooooooooooooo.          
                      .+oooooo+.    `:+oooooooooo.          
                     `/oooooooo:`      `:oooooooo.          
                     :soooooooo+`        ``/ooooo.          
                      :oooooooo-            `/soo.          
                       .--++--`                ./.  
";
    }
}

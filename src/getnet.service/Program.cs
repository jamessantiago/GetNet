using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using c = Colorful.Console;
using System.Drawing;

namespace getnet.service
{
    public class Program
    {
        private static Whistler logger = new Whistler(typeof(Program).FullName);

        public static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            string pathToCryptoKeys = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar
                    + "dp_keys" + Path.DirectorySeparatorChar;
            serviceCollection.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(pathToCryptoKeys));
            CoreCurrent.Protector = ActivatorUtilities.CreateInstance<DataProtect>(serviceCollection.BuildServiceProvider());

            CoreCurrent.Configuration["Whistler:Console:Enabled"] = "true";

            var service = false;
            var help = false;
            var conf = false;
            var syn = ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("c|configure", ref conf, "Configure the GetNet service");
                syntax.DefineOption("s|service", ref service, "Run as a service");
                syntax.DefineOption("h|help", ref help, "Show help");
            });
            if (help) {
                Console.WriteLine(HelpTextGenerator.Generate(syn, 80));
                Environment.Exit(0);
            }

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
                c.WriteLine(LOGO, Color.FromArgb(68, 138, 255));
                logger.Info("GetNet Service is starting up", WhistlerTypes.ServiceControl);
                Runner.Run();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine;

namespace getnet.service
{
    public class Program
    {
        private static Whistler logger = new Whistler(typeof(Program).FullName);

        public static void Main(string[] args)
        {
            CoreCurrent.Configuration["Whistler:Console:Enabled"] = "true";

            var service = false;
            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("s|service", ref service, "Run as a service");
            });

            if (service)
            {
                logger.Info("GetNet Service is starting up", WhistlerTypes.ServiceControl);
                Runner.Run();
            }
        }
    }
}

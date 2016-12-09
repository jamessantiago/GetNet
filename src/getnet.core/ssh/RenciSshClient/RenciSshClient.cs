using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace getnet.core.ssh
{
    public class RenciSshClient
    {
        private readonly IgetnetSshClientFactory gscFactory;
        private readonly IgetnetSshClientSettings gscSettings;
        
        public RenciSshClient(IgetnetSshClientSettings settings) : this(new RenciSshClientFactory(), settings)
        {
        }

        public RenciSshClient(IgetnetSshClientFactory factory, IgetnetSshClientSettings settings)
        {
            gscFactory = factory;
            gscSettings = settings;
        }

        public T Execute<T>(string command) where T : ICommandResult, new()
        {
            var client = this.gscFactory.CreateClient(this.gscSettings);
            var results = new T();
            try
            {
                var cmdResults = client.RunCommand(command);
                if (cmdResults.ExitStatus != 0)
                {
                    results = results.ConvertCommandResult<T>(cmdResults.Error, cmdResults.ExitStatus);
                } else
                {
                    results = results.ConvertCommandResult<T>(cmdResults.CommandText, 0);
                }
            } catch (Exception ex)
            {
                results = results.ConvertCommandResult<T>(ex.Message, 1);
            }
            return results;
        }        
    }
}

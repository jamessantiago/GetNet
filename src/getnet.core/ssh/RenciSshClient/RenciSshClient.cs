using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace getnet.core.ssh
{
    public class RenciSshClient : IGsc
    {
        private readonly IGscFactory gscFactory;
        private readonly IGscSettings gscSettings;
        
        public RenciSshClient(IGscSettings settings) : this(new RenciSshClientFactory(), settings)
        {
        }

        public RenciSshClient(IGscFactory factory, IGscSettings settings)
        {
            gscFactory = factory;
            gscSettings = settings;
        }

        public ICommandResult Execute<T>() where T : ICommandResult, new()
        {
            var command = new T();
            return this.Execute<T>(command.GetStoredCommand());
        }

        public ICommandResult Execute<T>(string command) where T : ICommandResult, new()
        {
            var client = this.gscFactory.CreateClient(this.gscSettings);
            ICommandResult results = new T();
            var cmdResults = client.RunCommand(command);
            if (cmdResults.ExitStatus != 0)
            {
                throw new Exception(cmdResults.Error);
            } else
            {
                results = results.ConvertCommandResult<T>(cmdResults.Result);
            }
            return results;
        }        
    }
}

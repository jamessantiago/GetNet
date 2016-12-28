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

        public List<T> Execute<T>() where T : ICommandResult, new()
        {
            var command = new T();
            return this.Execute<T>(command.GetStoredCommand());
        }

        public List<T> Execute<T>(string command) where T : ICommandResult, new()
        {
            var client = this.gscFactory.CreateClient(this.gscSettings);
            List<T> results = new List<T>();
            var executor = new T();
            var cmdResults = client.RunCommand(command);
            if (cmdResults.ExitStatus != 0)
            {
                throw new Exception(cmdResults.Error);
            } else
            {
                foreach (var result in executor.ConvertCommandResult<T>(cmdResults.Result))
                    results.Add((T)result);
            }
            return results;
        }        
    }
}

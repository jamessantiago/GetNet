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
        private Whistler logger = new Whistler();
        public SshCommand LastCommand;
        
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
            LastCommand = cmdResults;
            if (cmdResults.ExitStatus != 0)
            {
                logger.Error(
                    message: string.Format("Execution of command '{0}' against {1} resulted in exit status {2}: {3}", command, client.ConnectionInfo.Host, cmdResults.ExitStatus, cmdResults.Error),
                    type: WhistlerTypes.Ssh);
            } else
            {
                foreach (var result in executor.ConvertCommandResult<T>(cmdResults.Result))
                    results.Add((T)result);

                if (results.Count == 0)
                {
                    logger.Error(message: string.Format("Execution and parsing of command '{0}' against {1} yielded no results", command, client.ConnectionInfo.Host),
                        type: WhistlerTypes.Ssh,
                        details: cmdResults.Result);
                }
            }
            return results;
        }        
    }
}

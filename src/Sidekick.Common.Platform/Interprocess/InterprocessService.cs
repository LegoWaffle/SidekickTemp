using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipeMethodCalls;
using PipeMethodCalls.NetJson;
using Sidekick.Common;
using static System.Net.Mime.MediaTypeNames;

namespace Sidekick.Common.Platform.Interprocess
{
    public class InterprocessService: IInterprocessService
    {

        internal readonly ILogger<InterprocessService> logger;
        private static Action<string[]>? _CustomProtocolCallback;

        public InterprocessService(ILogger<InterprocessService> logger)
        {
            this.logger = logger;

        }

        public void CustomProtocolCallback(Action<string[]> callback)
        {
            _CustomProtocolCallback += callback;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                PipeServer<IInterprocessService> pipeServer;

                Random random = new Random();
                while (true)
                {
                    try
                    {
                        string pipeName = "sidekick-" + random.Next(0, 1000000).ToString();

                        using (StreamWriter f = File.CreateText(SidekickPaths.GetDataFilePath("pipename")))
                        {
                            f.Write(pipeName);
                        };

                        pipeServer = new PipeServer<IInterprocessService>(
                            new NetJsonPipeSerializer(),
                            pipeName,
                            () => new InterprocessService(logger));

                        await pipeServer.WaitForConnectionAsync();

                        File.Delete(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "pipename");

                        await pipeServer.WaitForRemotePipeCloseAsync();

                        pipeServer.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }

                }
            });
        }

        public void CustomProtocol(string[] args)
        {
            _CustomProtocolCallback?.Invoke(args);
        }

    }
}

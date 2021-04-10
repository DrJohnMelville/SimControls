using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Nerdbank.Streams;
using SimControls.NetworkCommon.NetworkVariableBinders;

namespace SimControls.NetworkServer
{
    public class SimServer
    {
        public string ClientAddress => server.PublicAddress;
        private readonly LocalBlazorServer server;
        private readonly Func<PipeReader, PipeWriter, INetworkVariableServer>
            serverFactory;

        public SimServer(LocalBlazorServer server,
            Func<PipeReader, PipeWriter, INetworkVariableServer> serverFactory)
        {
            this.server = server;
            this.serverFactory = serverFactory;
            GC.KeepAlive(WaitForConnectionsAsync());
        }

        private async Task WaitForConnectionsAsync()
        {
            await foreach (var socket in server.WaitForConnectionsAsync())
            {
                serverFactory(socket.UsePipeReader(), socket.UsePipeWriter());
            }
        }
    }
}
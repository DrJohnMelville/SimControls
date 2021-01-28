using System;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.P2P.Raw.Matchmaker;
using SimControls.Model;
using SimControls.NetworkCommon.NetworkVariableBinders;

namespace SimControls.NetworkServer
{
    public class SimServer
    {
        private readonly MatchmakerServer matchmaker;
        private readonly Func<IBinaryObjectPipeReader, IBinaryObjectPipeWriter, INetworkVariableServer>
            serverFactory;

        public SimServer(MatchmakerServer matchmaker, Func<IBinaryObjectPipeReader, IBinaryObjectPipeWriter, INetworkVariableServer> serverFactory)
        {
            this.matchmaker = matchmaker;
            this.serverFactory = serverFactory;
            MonitorForConnections();
        }

        private async void MonitorForConnections()
        {
            await foreach (var (reader, writer) in matchmaker.AcceptConnections())
            {
                serverFactory(reader, writer); // simply creating the server starts it.
            }
        }
    }
}
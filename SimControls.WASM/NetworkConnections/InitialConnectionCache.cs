using System;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.P2P.Raw.Matchmaker;
using Nerdbank.Streams;
using SimControls.Model;

namespace SimControls.WASM.NetworkConnections
{
    public interface IInitialConnectionCache
    {
        Task WaitForConnectionAsync();
        bool Connected { get; }
    }

    public class InitialConnectionCache: ISimVariableBinder, IInitialConnectionCache
    {
        private ISimVariableBinder? innerBinder;
        public bool Connected => innerBinder != null;
        private readonly Func<PipeReader, PipeWriter, ISimVariableBinder>
            binderFactory;

        public InitialConnectionCache(Func<PipeReader, PipeWriter, ISimVariableBinder> binderFactory)
        {
            this.binderFactory = binderFactory;
        }

        public void BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable) => 
            innerBinder?.BindVariableToSimulator(name, unit, simType, variable);

        public void BindEventToSimulator(SimEventTrigger simEvent) =>
            innerBinder?.BindEventToSimulator(simEvent);

        public async Task WaitForConnectionAsync()
        {
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri("ws://192.168.0.17:5432"), CancellationToken.None);
            innerBinder = binderFactory(socket.UsePipeReader(), socket.UsePipeWriter());
        }
    }
}
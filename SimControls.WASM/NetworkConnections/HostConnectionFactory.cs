using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SimControls.Model.VariableBinders;

namespace SimControls.WASM.NetworkConnections
{
    public class HostConnectionFactory
    {
        private readonly ICompositeVariableBinder rootBinder;
        private readonly Func<WebSocket, ISimVariableBinder>
            binderFactory;

        public HostConnectionFactory(ICompositeVariableBinder rootBinder, Func<WebSocket, ISimVariableBinder> binderFactory)
        {
            this.rootBinder = rootBinder;
            this.binderFactory = binderFactory;
        }

        public async Task ConnectToSeverAsync()
        {
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri("ws://192.168.0.17:5432"), CancellationToken.None);
            rootBinder.AddBinder(binderFactory(socket));
        }
    }
   }
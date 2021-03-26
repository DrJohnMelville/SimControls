using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SimControls.Model.VariableBinders;

namespace SimControls.WASM.NetworkConnections
{
    public class HostConnectionFactory
    {
        private readonly ICompositeVariableBinder rootBinder;
        private readonly Func<WebSocket, ISimVariableBinder>
            binderFactory;
        private readonly NavigationManager navMgr;

        public HostConnectionFactory(
            ICompositeVariableBinder rootBinder, 
            Func<WebSocket, ISimVariableBinder> binderFactory, 
            NavigationManager navMgr)
        {
            this.rootBinder = rootBinder;
            this.binderFactory = binderFactory;
            this.navMgr = navMgr;
        }

        public async Task ConnectToSeverAsync()
        {
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(HostWebSocketUri(), CancellationToken.None);
            rootBinder.AddBinder(binderFactory(socket));
        }

        private Uri HostWebSocketUri() => ChangeSchemeToWebSocket(navMgr.ToAbsoluteUri("/"));

        private static Uri ChangeSchemeToWebSocket(Uri host) =>
            new UriBuilder(host)
            {
                Scheme = "ws"
            }.Uri;
    }
   }
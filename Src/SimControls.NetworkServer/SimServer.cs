using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.P2P.Raw.Matchmaker;
using Nerdbank.Streams;
using SimControls.Model;
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

    public class LocalBlazorServer
    {
        public string PublicAddress { get; }
        private readonly HttpListener listener;

        public LocalBlazorServer()
        {
            PublicAddress = $"http://{LocalIp()}:5432/";
            listener = new HttpListener();
            // the shell command to allow normal users to do this is
            // netsh http add urlacl url="http://+:5432/" user=everyone
            listener.Prefixes.Add("http://+:5432/");
        }

        public async IAsyncEnumerable<WebSocket> WaitForConnectionsAsync()
        {
            listener.Start();
            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    yield return await OpenWebSocketAsync(context);
                }
                else
                {
                    await ServeStaticFileAsync(context);
                }
            }
        }

        private async Task ServeStaticFileAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            await using var output = response.OutputStream;
            await using var input = GetResponseStream(context.Request.Url?.AbsolutePath??"");
            await input.CopyToAsync(output);
        }

        private async Task<WebSocket> OpenWebSocketAsync(HttpListenerContext context)
        {
            var wsc = await context.AcceptWebSocketAsync(null!);
            return wsc.WebSocket;
        }

        private Stream GetResponseStream(string path)
        {
            if (path == "/") return GetResponseStream("/index.html");
            var fileName = @"C:\Users\jmelv\Documents\Scratch\WASM\wwwroot" + path;
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private static string LocalIp() =>
            Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(i => i.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
    }
}
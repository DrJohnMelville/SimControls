using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SimControls.NetworkServer
{
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

        private static string LocalIp() =>
            Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(i => i.AddressFamily == AddressFamily.InterNetwork)
                .ToString();

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
            try
            {
                await using var input = GetResponseStream(context.Request.Url?.AbsolutePath??"");
                await input.CopyToAsync(output);
            }
            catch (IOException)
            {
            }
        }

        private async Task<WebSocket> OpenWebSocketAsync(HttpListenerContext context)
        {
            var wsc = await context.AcceptWebSocketAsync(null!);
            return wsc.WebSocket;
        }

        private Stream GetResponseStream(string path)
        {
            var fileName = WebToLocalPath(CheckForDefaultPath(path));
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private static string CheckForDefaultPath(string path) => path == "/"?"/index.html":path;

        private static string WebToLocalPath(string path) => 
            $@"{AppDomain.CurrentDomain.BaseDirectory}\WASM\wwwroot{path}";
    }
}
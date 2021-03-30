using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.FileSystem;

namespace SimControls.NetworkServer
{
    public class LocalBlazorServer
    {
        public string PublicAddress { get; }
        private readonly HttpListener listener;
        private readonly LocalFileMapper fileMapper;

        public LocalBlazorServer(LocalFileMapper fileMapper)
        {
            this.fileMapper = fileMapper;
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

        private async Task<WebSocket> OpenWebSocketAsync(HttpListenerContext context)
        {
            var wsc = await context.AcceptWebSocketAsync(null!);
            return wsc.WebSocket;
        }

        private  Task ServeStaticFileAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            var path = CheckForDefaultPath(context.Request.Url?.AbsolutePath??"");
            var (responseFile, encoding) = 
                fileMapper.GetResponseFile(path, context.Request.Headers.Get("Accept-Encoding"));
            if (responseFile == null) return Task.CompletedTask;
            response.ContentType = MimeMap.FromPath(path);
            if (encoding != null) response.Headers.Add("Content-Encoding", encoding);

            return DoCopyOperationAsync(responseFile, response);
        }

        private static async Task DoCopyOperationAsync(IFile responseFile, HttpListenerResponse response)
        {
            try
            {
                await using var input = await responseFile.OpenRead();
                await using var output = response.OutputStream;
                await input.CopyToAsync(output);
            }
            catch (IOException)
            {
            }
        }

        private static string CheckForDefaultPath(string path) => path == "/"?"/index.html":path;
    }
}
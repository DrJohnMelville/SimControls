using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.P2P.Raw.Matchmaker;
using SimControls.Model;
using SimControls.NetworkCommon.NetworkVariableBinders;

namespace SimControls.NetworkServer
{
    public class SimServer
    {
        public string ClientAddress => server.PublicAddress;
        private readonly LocalBlazorServer server;
        private readonly Func<IBinaryObjectPipeReader, IBinaryObjectPipeWriter, INetworkVariableServer>
            serverFactory;

        public SimServer(LocalBlazorServer server,
            Func<IBinaryObjectPipeReader, IBinaryObjectPipeWriter, INetworkVariableServer> serverFactory)
        {
            this.server = server;
            this.serverFactory = serverFactory;
        }
    }

    public class LocalBlazorServer
    {
        public string PublicAddress { get; }
        private HttpListener listener;

        public LocalBlazorServer()
        {
            PublicAddress = $"http://{LocalIp()}:5432/";
            InitializeHttpContext();
        }

        private async void InitializeHttpContext()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://+:80/Temporary_Listen_Addresses/SimControls/");
            listener.Start();
            while (true)
            {
                var context = await listener.GetContextAsync();
                HttpListenerResponse response = context.Response;
                // Construct a response.
                var rnd = new Random();
                string responseString = $"<HTML><BODY> Hello world! {rnd.Next()}</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                await output.WriteAsync(buffer,0,buffer.Length);
                // You must close the output stream.
                output.Close();
            }
        }

        private static string LocalIp() =>
            Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(i => i.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
    }
}
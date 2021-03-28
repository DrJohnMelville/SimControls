using System;
using System.Net.Http;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Melville.IOC.AspNet.RegisterFromServiceCollection;
using Melville.P2P.Raw.BinaryObjectPipes;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nerdbank.Streams;
using SimControls.Model;
using SimControls.Model.VariableBinders;
using SimControls.NetworkCommon.DataClasses;
using SimControls.NetworkCommon.NetworkVariableBinders;
using SimControls.WASM.NetworkConnections;

namespace SimControls.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                .CreateDefault(args);
            
            builder.ConfigureContainer(new MelvilleServiceProviderFactory(true, ioc =>
                new Startup(ioc, builder.HostEnvironment).Configure()));
            builder.RootComponents.Add<App>("#app");

            await builder.Build().RunAsync();
        }

    }
}

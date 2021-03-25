using System;
using System.Net.Http;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            RegisterVariableCache(builder.Services);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }

        private static void RegisterVariableCache(IServiceCollection svc)
        {
            BinaryObjectDictionary dict = new SimObjectDictionary();
            svc.AddSingleton<BinaryObjectDictionary>(dict);
            svc.AddSingleton<ISimVariableBinder, InitialConnectionCache>();
            svc.AddSingleton<IInitialConnectionCache>(i =>
                (IInitialConnectionCache) i.GetRequiredService<ISimVariableBinder>());
            svc.AddSingleton<IVariableCache, VariableCache>();
            svc.AddSingleton<Func<PipeReader, PipeWriter, ISimVariableBinder>>(
                (reader,writer) => 
                    new NetworkVariableBinder(
                        new BinaryObjectPipeReader(reader, dict), 
                        new BinaryObjectPipeWriter(writer, dict)));
        }
    }
}

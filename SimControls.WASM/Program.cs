using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.P2P.Raw.Matchmaker;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimControls.Model;
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

using System;
using System.Net.Http;
using System.Collections.Generic;
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
            svc.AddSingleton<BinaryObjectDictionary, SimObjectDictionary>();
            svc.AddSingleton<ISimVariableBinder, InitialConnectionCache>();
            svc.AddSingleton<IInitialConnectionCache>(i =>
                (IInitialConnectionCache) i.GetRequiredService<ISimVariableBinder>());
            svc.AddSingleton<IVariableCache, VariableCache>();
            svc.AddSingleton<Func<BinaryObjectPipeReader, BinaryObjectPipeWriter, ISimVariableBinder>>(
                (reader,writer) => new NetworkVariableBinder(reader, writer));
            svc.AddTransient<MatchMakerClient>(i =>
                new MatchMakerClient(70, i.GetRequiredService<BinaryObjectDictionary>()));
        }
    }
}

using System.Threading.Tasks;
using Melville.IOC.AspNet.RegisterFromServiceCollection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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

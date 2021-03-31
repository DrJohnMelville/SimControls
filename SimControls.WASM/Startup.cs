using System;
using System.Net.Http;
using System.Net.WebSockets;
using Melville.IOC.IocContainers;
using Melville.P2P.Raw.BinaryObjectPipes;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Nerdbank.Streams;
using SimControls.Model;
using SimControls.Model.VariableBinders;
using SimControls.NetworkCommon.DataClasses;
using SimControls.NetworkCommon.NetworkVariableBinders;

namespace SimControls.WASM
{
    public class Startup
    {
        private readonly IBindableIocService ioc;
        private readonly IWebAssemblyHostEnvironment hostEnvironment;

        public Startup(IBindableIocService ioc, IWebAssemblyHostEnvironment hostEnvironment)
        {
            this.ioc = ioc;
            this.hostEnvironment = hostEnvironment;
        }

        public void Configure()
        {
            ConfigureServerConnection();
            ioc.Bind<ICompositeVariableBinder>()
                .And<ISimVariableBinder>()
                .To<CompositeVariableBinder>().AsSingleton();
            ioc.Bind<IVariableCache>().To<VariableCache>().AsSingleton();
        }

        private void ConfigureServerConnection()
        {
            ioc.Bind<HttpClient>()
                .ToMethod(i=>new HttpClient{BaseAddress = new Uri(hostEnvironment.BaseAddress)})
                .AsSingleton();
            var dict = new SimObjectDictionary();
            ioc.Bind<BinaryObjectDictionary>().ToConstant(dict);
            ioc.Bind<Func<WebSocket, ISimVariableBinder>>().ToConstant(s =>
                new NetworkVariableBinder(
                    new BinaryObjectPipeReader(s.UsePipeReader(), dict),
                    new BinaryObjectPipeWriter(s.UsePipeWriter(), dict))
            );
        }
    }
}
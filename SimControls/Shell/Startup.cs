using System;
using System.IO;
using System.IO.Pipelines;
using System.Windows;
using Melville.IOC.BindingRequests;
using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies.TypeActivation;
using Melville.MVVM.USB;
using Melville.MVVM.WindowMessages;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.WindowMessages;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.WpfAppFramework.StartupBases;
using SimControls.AirportDatabase;
using SimControls.Model;
using SimControls.Model.AirportDatabase;
using SimControls.Model.VariableBinders;
using SimControls.NetworkCommon.DataClasses;
using SimControls.NetworkCommon.NetworkVariableBinders;
using SimControls.SimulatorConnection;
using SimControls.YokeConnector;
using SimVariableList = SimControls.SimulatorConnection.SimVariableList;

namespace SimControls.Shell
{
    public sealed class Startup:StartupBase
    {
        [STAThread]
        public static int Main(string[] commandLineArgs)
        {
            ApplicationRootImplementation.Run(new Startup());
            
            return 0;
        }

        protected override void RegisterWithIocContainer(IBindableIocService service)
        {
            service.AddLogging();
            RegisterWindows(service);
            RegisterDataStore(service);
            RegisterHardware(service);
            RegisterAirportDatabase(service);
            RegisterTabletServer(service);
            RegisterDebugger(service);
        }

        private void RegisterDebugger(IBindableIocService service)
        {
            service.Bind<Func<object, IRootNavigationWindow>>().ToConstant(o =>
            {
                var navWindow = new NavigationWindow();
                navWindow.NavigateTo(o);
                return new RootNavigationWindow(navWindow);
            });
        }

        private void RegisterTabletServer(IBindableIocService service)
        {
            service.Bind<BinaryObjectDictionary>().To<SimObjectDictionary>().AsSingleton();
            service.Bind<IBinaryObjectPipeReader>().To<BinaryObjectPipeReader>(
                i => i.WithArgumentTypes<PipeReader, BinaryObjectDictionary>());
            service.Bind<IBinaryObjectPipeWriter>().To<BinaryObjectPipeWriter>(
                i => i.WithArgumentTypes<PipeWriter, BinaryObjectDictionary>());
            service.Bind<Func<PipeReader, PipeWriter, INetworkVariableServer>>()
                .ToMethod(CreateNetworkVariableServer);
        }

        private Func<PipeReader, PipeWriter, INetworkVariableServer> 
            CreateNetworkVariableServer(IIocService ioc, IBindingRequest br)
        {
            return (r, w) =>
            {
                var dict = ioc.Get<BinaryObjectDictionary>();
                return new NetworkVariableServer(ioc.Get<IVariableCache>(),
                    new BinaryObjectPipeReader(r, dict), new BinaryObjectPipeWriter(w, dict));
            };
        }

        private static void RegisterDataStore(IBindableIocService service)
        {
            service.Bind<ISimulatorInterface>().To<SimulatorInterface>().AsSingleton();
            service.Bind<ISimVariableBinder>().And<SimVariableList>().To<SimVariableList>().AsSingleton();
            service.Bind<IVariableCache>().To<VariableCache>().AsSingleton();
        }

        private static void RegisterWindows(IBindableIocService service)
        {
            service.RegisterHomeViewModel<RootViewModel>();
            service.Bind<INavigationWindow>().To<NavigationWindow>().AsSingleton();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .AsSingleton();
            service.Bind<IWindowMessageSource>().To<WindowMessageSource>().AsSingleton();
        }

        private static void RegisterAirportDatabase(IBindableIocService service)
        {
            service.Bind <Func<AirportDbContext>>().ToConstant(AirportContext());
            service.Bind<IAirportRepository>().To<AirportSqlRepository>();
        }

        private static Func<AirportDbContext> AirportContext()
        {
            var fileName = Path.Join(Path.GetDirectoryName(typeof(AirportDbContext).Assembly.Location),
                "Airports.db");
            return AirportDbContext.DataContextFactory(fileName);
        }

        private static void RegisterHardware(IBindableIocService service)
        {
            service.Bind<IYokeConnection>().To<YokeConnection>().AsSingleton();
            service.Bind<IMonitorForDeviceArrival>().To<MonitorForDeviceArrival>().DisposeIfInsideScope();
        }
    }
}
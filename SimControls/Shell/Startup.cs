using System;
using System.IO;
using System.Windows;
using Melville.IOC.IocContainers;
using Melville.MVVM.USB;
using Melville.MVVM.WindowMessages;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.WindowMessages;
using Melville.WpfAppFramework.StartupBases;
using SimControls.AirportDatabase;
using SimControls.Model;
using SimControls.Model.AirportDatabase;
using SimControls.SimulatorConnection;
using SimControls.YokeConnector;

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
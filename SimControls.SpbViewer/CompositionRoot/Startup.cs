using Melville.IOC.IocContainers;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.WindowMessages;
using Melville.SystemInterface.WindowMessages;
using Melville.WpfAppFramework.StartupBases;
using System;
using System.Windows;
using Melville.MVVM.Wpf.MvvmDialogs;
using SimControls.SpbViewer.DefaultPropDefs;
using SimControls.SpbViewer.PropertyAndSetDeclarations;
using SimControls.SpbViewer.Views;

namespace SimControls.SpbViewer.CompositionRoot;

public sealed class Startup : StartupBase
{
    [STAThread]
    public static void Main()
    {
        ApplicationRootImplementation.Run(new Startup());
    }

    protected override void RegisterWithIocContainer(IBindableIocService service)
    {
        service.AddLogging();
        service.RegisterHomeViewModel<RootViewModel>();
        service.Bind<INavigationWindow>().To<NavigationWindow>().AsSingleton();
        service.Bind<IRootNavigationWindow>()
            .And<Window>()
            .To<RootNavigationWindow>()
            .AsSingleton();
        service.Bind<IWindowMessageSource>().To<WindowMessageSource>().AsSingleton();

        service.Bind<IOpenSaveFile>().To<OpenSaveFileAdapter>();
        service.Bind<Func<Task<IPropertyRegistry>>>()
            .ToConstant(DefaultPropertyLibrary.GlobalRegistryAsync);

    }
}
using System;
using Melville.MVVM.Wpf.RootWindows;
using SimControls.NetworkServer;
using SimControls.YokeConnector;

namespace SimControls.Shell
{
    public class RootViewModel
    {
        public SimServer Server { get; }
        private readonly INavigationWindow navWindow;
        public RootViewModel(INavigationWindow navWindow, IYokeConnection yoke, SimServer server)
        {
            Server = server;
            this.navWindow = navWindow;
            // Yoke and server are singletons, we demand that they exist at this point and then
            // the IOC framework will keep the active.
        }

        private void NavigateToThisPage(object? sender, EventArgs e) => navWindow.NavigateTo(this);
    }
}
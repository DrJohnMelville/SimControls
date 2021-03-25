using System;
using System.Threading.Tasks;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using SimControls.Debuggers;
using SimControls.FlightDisplays;
using SimControls.Model;
using SimControls.NetworkServer;
using SimControls.SimulatorConnection;
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
        public async Task ConnectToSimAsync(IWaitingService wait,
            [FromServices] ISimulatorInterface simulator,
            [FromServices] Func<FlightDisplayViewModel> displayFactory)
        {
            using (wait.WaitBlock("Connecting to Simulator"))
            {
                try
                {
                    await simulator.Connect();
                    simulator.LostConnection += NavigateToThisPage;
                    FlightDisplay(displayFactory);
                }
                catch (Exception e)
                {
                    wait.ErrorMessage = e.Message;
                    throw;
                }
            }
        }

        public void LaunchDebugger([FromServices]Func<object, IRootNavigationWindow> winFactory,
            [FromServices]DebuggerViewModel debuggerViewModel)
        {
            winFactory(debuggerViewModel).Show();
        }

        public void FlightDisplay([FromServices]Func<FlightDisplayViewModel> displayFactory)
        {
            navWindow.NavigateTo(displayFactory());
        }

        private void NavigateToThisPage(object? sender, EventArgs e) => navWindow.NavigateTo(this);
    }
}
using System;
using System.Threading.Tasks;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using SimControls.FlightDisplays;
using SimControls.SimulatorConnection;
using SimControls.YokeConnector;

namespace SimControls.Shell
{
    public class RootViewModel
    {
        private readonly INavigationWindow navWindow;
       // private readonly IVoiceCommand voice; // just holding this reference keeps the voice command working
       private IYokeConnection yoke; // create the singleton and it will stay
        public RootViewModel(INavigationWindow navWindow, IYokeConnection yoke)
        {
            this.navWindow = navWindow;
            this.yoke = yoke;
        }

        public async Task ConnectToSim(IWaitingService wait,
            [FromServices] ISimulatorInterface simulator,
            [FromServices] Func<FlightDisplayViewModel> displayFactory)
        {
            using (wait.WaitBlock("Connecting to Simulator"))
            {
                try
                {
                    await simulator.Connect();
                    simulator.LostConnection += NavigateToThisPage;
                    navWindow.NavigateTo(displayFactory());
                }
                catch (Exception e)
                {
                    wait.ErrorMessage = e.Message;
                    throw;
                }
            }
        }

        private void NavigateToThisPage(object? sender, EventArgs e) => navWindow.NavigateTo(this);
    }
}
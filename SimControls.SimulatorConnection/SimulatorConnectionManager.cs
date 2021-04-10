using System;
using System.Threading.Tasks;
using Melville.SystemInterface.WindowMessages;
using Microsoft.FlightSimulator.SimConnect;
using SimControls.Model.VariableBinders;

namespace SimControls.SimulatorConnection
{
    public partial class SimulatorConnectionManager : ISimulatorInterface
    {
        private readonly IWindowMessageSource msgSrc;
        private readonly ICompositeVariableBinder rootBinder;
        public ISimulatorCommandTarget CommandTarget { get; set; } =
            new NullSimulatorCommandTarget();

        public SimulatorConnectionManager(IWindowMessageSource msgSrc, ICompositeVariableBinder rootBinder)
        {
            this.msgSrc = msgSrc;
            this.rootBinder = rootBinder;
        }

        public Task Connect()
        {
            TaskCompletionSource<int> tcs = new();
            try
            {
                var connection = new SimConnect("SimControls", msgSrc.SourceWindowHWnd, 0x402, null, 0);
                connection.OnRecvOpen += (s, e) => tcs.SetResult(1);
                RegisterQuitMessage(connection);
                RegisterForWindowMethod(connection);
                SetConnectedSimulator(connection);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }

            return tcs.Task;
        }

        private void SetConnectedSimulator(SimConnect connection)
        {
            var connectedSimulator = new ConnnectedSimulator(connection);
            CommandTarget = connectedSimulator;
            rootBinder.AddBinder(connectedSimulator);
        }

        private void RegisterQuitMessage(SimConnect connection)
        {
            void OnRecvQuit(SimConnect s, SIMCONNECT_RECV e)
            {
                connection.OnRecvQuit -= OnRecvQuit;
                CommandTarget = new NullSimulatorCommandTarget();
                LostConnection?.Invoke(this, EventArgs.Empty);
            }

            connection.OnRecvQuit += OnRecvQuit;
        }

        private void RegisterForWindowMethod(SimConnect connection)
        {
            msgSrc.RegisterForMessage(0x402).MessageReceived += (s, e) => connection.ReceiveMessage();
        }


        public event EventHandler<EventArgs>? LostConnection;
    }
}
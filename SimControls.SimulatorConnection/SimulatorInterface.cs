using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Melville.MVVM.WindowMessages;
using Microsoft.FlightSimulator.SimConnect;
using SimControls.Model;

namespace SimControls.SimulatorConnection
{
    public interface ISimulatorInterface
    {
        public Task Connect();
        public void LoadFlightPlan(string plan);
        public event EventHandler<EventArgs> LostConnection;
    }

    public class SimulatorInterface : ISimulatorInterface
    {
        private SimConnect? connection;
        private readonly IWindowMessageSource msgSrc;
        private readonly SimVariableList variables;

        public SimulatorInterface(IWindowMessageSource msgSrc, SimVariableList variables)
        {
            this.msgSrc = msgSrc;
            this.variables = variables;
        }

        public Task Connect()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            try
            {
                var eventHolder = msgSrc.RegisterForMessage(0x402);
                eventHolder.MessageReceived += (s, e) => connection?.ReceiveMessage();
                connection = new SimConnect("SimControls", msgSrc.SourceWindowHWnd, 0x402, null, 0);
                connection.OnRecvOpen += (s, e) => tcs.SetResult(1);
                connection.OnRecvQuit += SendQuitEvent;
                RegisterDataReceivedEvent();
                variables.InjectSimRequestCallback(RegisterVariable, RegisterSimEvent);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }

            return tcs.Task;
        }

        public void RegisterDataReceivedEvent()
        {
            if (connection == null) return;
            connection.OnRecvSimobjectDataBytype += DataReceived;
        }

        private async void DataReceived(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            variables.UpdateVariables(data.dwRequestID, data.dwData[0]);
            await Task.Delay(200);
            SubmitSimRequest((FakeEnum) data.dwRequestID);
        }

        private void SubmitSimRequest(Enum eventRequest)
        {
            connection?.RequestDataOnSimObjectType(eventRequest, eventRequest,
                0, SIMCONNECT_SIMOBJECT_TYPE.USER);
        }

        private void RegisterVariable(ISimVariable variable)
        {
            if (connection == null) return;
            variable.Declare(connection);
            SubmitSimRequest(variable.EventEnum);
        }


        public event EventHandler<EventArgs>? LostConnection;

        private void SendQuitEvent(SimConnect sender, SIMCONNECT_RECV data)
        {
            connection?.Dispose();
            connection = null;
            LostConnection?.Invoke(this, EventArgs.Empty);
        }

        public void LoadFlightPlan(string plan)
        {
            File.WriteAllText(@"D:\Users\John\Desktop\Scratch\Temp.PLN", plan);
            connection?.FlightPlanLoad(@"D:\Users\John\Desktop\Scratch\temp.PLN");
        }

        private void RegisterSimEvent(SimEventTrigger simEvent)
        {
            if (connection == null) return;
            connection.MapClientEventToSimEvent((FakeEnum) simEvent.SimEventNumber, simEvent.SimulatorEventName);
            simEvent.RegisterEffector(FireSimEvent);
        }

        private void FireSimEvent(int eventNum, uint data)
        {
            connection?.TransmitClientEvent(0u, (FakeEnum)eventNum, data, FakeEnum.Foo,
                SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}
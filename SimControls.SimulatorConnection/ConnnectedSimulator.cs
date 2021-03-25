using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.FlightSimulator.SimConnect;
using SimControls.Model;
using SimControls.Model.VariableBinders;

namespace SimControls.SimulatorConnection
{
    public sealed class ConnnectedSimulator : ISimulatorCommandTarget, ISimVariableBinder
    {
        private readonly SimConnect connection;
        private readonly SimVariableList variables = new();
        
        public ConnnectedSimulator(SimConnect connection)
        {
            this.connection = connection;
            connection.OnRecvSimobjectDataBytype += DataReceived;
        }

        #region Load Flight Plans

        public void LoadFlightPlan(string plan)
        {
            File.WriteAllText(@"D:\Users\John\Desktop\Scratch\Temp.PLN", plan);
            connection.FlightPlanLoad(@"D:\Users\John\Desktop\Scratch\temp.PLN");
        }

        #endregion

        #region Variable binding

        public void BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable)
        {
            var boundVar = variables.BindVariableToSimulator(name, unit, simType, variable);
            boundVar.Declare(connection);
            SubmitSimRequest(boundVar.EventEnum);
        }
        
        private async void DataReceived(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            variables.UpdateVariables(data.dwRequestID, data.dwData[0]);
            await Task.Delay(200);
            SubmitSimRequest((FakeEnum) data.dwRequestID);
        }

        private void SubmitSimRequest(Enum eventRequest)
        {
            connection.RequestDataOnSimObjectType(eventRequest, eventRequest,
                0, SIMCONNECT_SIMOBJECT_TYPE.USER);
        }

        #endregion

        #region Register Events

        public void BindEventToSimulator(SimEventTrigger simEvent)
        {
            connection.MapClientEventToSimEvent((FakeEnum) simEvent.SimEventNumber, simEvent.SimulatorEventName);
            simEvent.RegisterEffector(FireSimEvent);
        }
        private void FireSimEvent(int eventNum, uint data) =>
            connection.TransmitClientEvent(0u, (FakeEnum)eventNum, data, FakeEnum.Foo,
                SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);

        #endregion
    }
    
    public class SimVariableList
    {
        private List<ISimVariable> variables = new();
        
        public void UpdateVariables(uint eventRequest, object planeInfo)
        {
            variables[(int)eventRequest].UpdateVariable(planeInfo);
        }
        
        public ISimVariable BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable)
        {
            var simVariable = new SimVariable<T>(name, unit,
                ToEnumDataType<T>(simType), variable, variables.Count);
            variables.Add(simVariable);
            return simVariable;
        }
        private static SIMCONNECT_DATATYPE ToEnumDataType<T>(string simType) => 
            Enum.Parse<SIMCONNECT_DATATYPE>(simType);

    }

}
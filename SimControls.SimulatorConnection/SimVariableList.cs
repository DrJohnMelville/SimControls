using System;
using System.Collections.Generic;
using Microsoft.FlightSimulator.SimConnect;
using SimControls.Model;
using SimControls.Model.VariableBinders;

namespace SimControls.SimulatorConnection
{
    public class SimVariableList2
    {
        private List<ISimVariable> variables = new();
        
        public void UpdateVariables(uint eventRequest, object planeInfo)
        {
            variables[(int)eventRequest].UpdateVariable(planeInfo);
        }
    }
    public class SimVariableList: ISimVariableBinder
    {
        private List<ISimVariable> variables = new();
        private Action<ISimVariable> bindToSimulator = GC.KeepAlive;
       
        public SimVariableList()
        {
            registerEvent = preRegisteredEvents.Add;
        }

        public void InjectSimRequestCallback(
            Action<ISimVariable> method, Action<SimEventTrigger> eventMethod)
        {
            bindToSimulator = method;
            foreach (var variable in variables)
            {
                bindToSimulator(variable);
            }

            registerEvent = eventMethod;
            foreach (var simEvent in preRegisteredEvents)
            {
                registerEvent(simEvent);
            }
            preRegisteredEvents.Clear();
        }

        public void UpdateVariables(uint eventRequest, object planeInfo)
        {
                variables[(int)eventRequest].UpdateVariable(planeInfo);
        }

        public void BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable)
        {
            var simVariable = new SimVariable<T>(name, unit,
                ToEnumDataType<T>(simType), variable, variables.Count);
            variables.Add(simVariable);
            bindToSimulator(simVariable);
        }

        private static SIMCONNECT_DATATYPE ToEnumDataType<T>(string simType) => 
            Enum.Parse<SIMCONNECT_DATATYPE>(simType);

        #region Events

        private List<SimEventTrigger> preRegisteredEvents = new();
        private Action<SimEventTrigger> registerEvent; 
        public void BindEventToSimulator(SimEventTrigger simEvent)
        {
            registerEvent(simEvent);
        }

        #endregion
    }
}
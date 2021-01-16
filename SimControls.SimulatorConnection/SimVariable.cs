using System;
using Microsoft.FlightSimulator.SimConnect;
using SimControls.Model;

namespace SimControls.SimulatorConnection
{
    public interface ISimVariable
    {
        void UpdateVariable(object info);
        void Declare(SimConnect conn);
        Enum EventEnum { get; }
    }
    public readonly struct DoubleWrapper
    {
        public readonly double value;
        public DoubleWrapper(double value)
        {
            this.value = value;
        }
    }
    public readonly struct IntWrapper
    {
        public readonly int value;
        public IntWrapper(int value)
        {
            this.value = value;
        }
    }

    public enum FakeEnum
    {
        Foo
    };

    public class SimVariable<T>: ISimVariable
    {
        private readonly string name;
        private readonly string unitName;
        private readonly SIMCONNECT_DATATYPE simType;
        private readonly ReadOnlyDataItem<T> target;
        public Enum EventEnum { get; }

        public SimVariable(
            string name, string unitName, SIMCONNECT_DATATYPE simType, 
            ReadOnlyDataItem<T> target, int eventRequest)
        {
            this.name = name;
            this.unitName = unitName;
            this.simType = simType;
            this.target = target;
            this.EventEnum = (FakeEnum)eventRequest;
        }
        
        public void UpdateVariable(object info) => target.TryUpdateFromSimulator((T)GetValue(info));

        private object GetValue(object info)
        {
            return info switch
            {
                DoubleWrapper dw => (object)dw.value,
                IntWrapper iw => (object)iw.value,
                _ => throw new InvalidOperationException("Unknown type")
            };
        }


        public virtual void Declare(SimConnect conn)
        {
            connection = conn;
            conn.AddToDataDefinition(EventEnum, name, unitName, simType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            CreateVariableCollectionStuct(conn);
            if (target is DataItem<T> writableTarget)
            {
                writableTarget.WriteValueToSimulator += Write;
            }
        }

        private void CreateVariableCollectionStuct(SimConnect conn)
        {
            switch (target.Value)
            {
                case double d:
                    conn.RegisterDataDefineStruct<DoubleWrapper>(EventEnum);
                    break;
                case int i:
                    conn.RegisterDataDefineStruct<IntWrapper>(EventEnum);
                    break;
            }
        }

        protected SimConnect? connection;

        public virtual void Write(object? o, EventArgs e)
        {
            var output= CreateTransferObject();
            connection?.SetDataOnSimObject(EventEnum, SimConnect.SIMCONNECT_OBJECT_ID_USER,
                SIMCONNECT_DATA_SET_FLAG.DEFAULT, output);
        }

        private object CreateTransferObject() =>
            target.Value switch
            {
                double d => new DoubleWrapper(d),
                int i => new IntWrapper(i),
                _ => throw new InvalidOperationException("Unknowwn type")
            };
    }

    public class SimVariableWithEvent<T> : SimVariable<T>
    {
        private readonly string setEventName;

        public SimVariableWithEvent(
            string name, string unitName, SIMCONNECT_DATATYPE simType, DataItem<T> target, 
            int eventRequest, string setEventName) :
            base(name, unitName, simType, target, eventRequest)
        {
            this.setEventName = setEventName;
        }

        public override void Declare(SimConnect conn)
        {
            base.Declare(conn);
            conn.MapClientEventToSimEvent(EventEnum, setEventName);
        }

        public override void Write(object? o, EventArgs e)
        {
            connection?.TransmitClientEvent(0u, EventEnum, 1, FakeEnum.Foo, 
                SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using Melville.MVVM.AdvancedLists;
using SimControls.Model;
using SimControls.Model.VariableBinders;

namespace SimControls.Debuggers
{
    public record DebugVariableLine (string Name, string Unit, string SimType, DataItem Item)
    {
    }
    public class DebuggerViewModel: ISimVariableBinder
    {
        public IList<DebugVariableLine> Variables { get; } =
            new ThreadSafeBindableCollection<DebugVariableLine>();

        public IList<SimEventTrigger> Events { get; } =
            new ThreadSafeBindableCollection<SimEventTrigger>();

        public IList<string> Console { get; }=
            new ThreadSafeBindableCollection<string>();

        public DebuggerViewModel(ICompositeVariableBinder binder)
        {
            binder.AddBinder(this);
        }

        #region Register Data Items

        public void BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable)
        {
            Variables.Add(new DebugVariableLine(name, unit, simType, variable));
        }

        public void BindEventToSimulator(SimEventTrigger simEvent)
        {
            Events.Add(simEvent);
            simEvent.RegisterEffector((_,data)=>
                Console.Insert(0,$"{simEvent.SimulatorEventName}: {data}"));
        }

        public void FireEvent(SimEventTrigger trigger) => trigger.Fire(1);

        #endregion
    }
}
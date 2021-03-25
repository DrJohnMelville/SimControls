using System.Collections.Generic;

namespace SimControls.Model.VariableBinders
{
    public interface ISimVariableBinder
    {
        void BindVariableToSimulator<T>(string name, string unit, string simType, ReadOnlyDataItem<T> variable);
        void BindEventToSimulator(SimEventTrigger simEvent);
    }

    public interface ICompositeVariableBinder : ISimVariableBinder
    {
        public void AddBinder(ISimVariableBinder newBinder);
    }

    public class CompositeVariableBinder: ICompositeVariableBinder
    {
        private List<ISimVariableBinder> binders = new();
        private BindingOperationJournal journal = new();

        public CompositeVariableBinder()
        {
            binders.Add(journal);
        }

        public void BindVariableToSimulator<T>(string name, string unit, string simType, ReadOnlyDataItem<T> variable)
        {
            foreach (var target in binders)
            {
                target.BindVariableToSimulator(name, unit, simType, variable);
            }
        }

        public void BindEventToSimulator(SimEventTrigger simEvent)
        {
            foreach (var target in binders)
            {
                target.BindEventToSimulator(simEvent);
            }
        }

        public void AddBinder(ISimVariableBinder newBinder)
        {
            journal.ReplayPriorRegistrations(newBinder);
            binders.Add(newBinder);
        }
    }
}
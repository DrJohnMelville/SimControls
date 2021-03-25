using System;
using System.Collections.Generic;

namespace SimControls.Model.VariableBinders
{
    public sealed class BindingOperationJournal : ISimVariableBinder
    {
        private readonly List<Action<ISimVariableBinder>> rebindingActions = new();

        public void BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable) =>
            rebindingActions.Add(i => i.BindVariableToSimulator(name, unit, simType, variable));

        public void BindEventToSimulator(SimEventTrigger simEvent) =>
            rebindingActions.Add(i => i.BindEventToSimulator(simEvent));

        public void ReplayPriorRegistrations(ISimVariableBinder newBinder)
        {
            foreach (var rebindingAction in rebindingActions)
            {
                rebindingAction(newBinder);
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace SimControls.Model
{
    public static class EventCounter
    {
        private static int nextEventNumber;
        public static int Next() => nextEventNumber++;
    }

    public interface ISimEventTrigger
    {
        void Fire(uint data = 1);
    }

    public class SimEventTrigger : ISimEventTrigger
    {
        public string SimulatorEventName { get; }
        public int SimEventNumber { get; }
        private List<Action<int, uint>> effectors = new();

        public SimEventTrigger(string simulatorEventName)
        {
            SimulatorEventName = simulatorEventName;
            SimEventNumber = EventCounter.Next();
        }

        public void RegisterEffector(Action<int, uint> newEffector) => effectors.Add(newEffector);

        public void Fire(uint data = 1)
        {
            foreach (var effector in effectors)
            {
                effector.Invoke(SimEventNumber, data);
            }
        }
    }
}
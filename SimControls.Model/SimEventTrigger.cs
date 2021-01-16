using System;

namespace SimControls.Model
{
    public static class EventCounter
    {
        private static int nextEventNumber;
        public static int Next() => nextEventNumber++;
    }
    public class SimEventTrigger
    {
        public string SimulatorEventName { get; }
        public int SimEventNumber { get; }
        private Action<int, uint>? effector;

        public SimEventTrigger(string simulatorEventName)
        {
            SimulatorEventName = simulatorEventName;
            SimEventNumber = EventCounter.Next();
        }

        public void RegisterEffector(Action<int, uint> newEffector) => effector = newEffector;

        public void Fire(uint data = 1)
        {
            effector?.Invoke(SimEventNumber, data);
        }
    }
}
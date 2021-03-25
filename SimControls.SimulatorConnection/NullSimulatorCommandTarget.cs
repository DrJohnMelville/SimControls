using System;
using System.Threading.Tasks;

namespace SimControls.SimulatorConnection
{
    public class NullSimulatorCommandTarget : ISimulatorCommandTarget
    {
        public void LoadFlightPlan(string plan)
        {
        }
    }
}
using System;
using Melville.Hacks;
using Melville.SystemInterface.USB.Joysticks;
using SimControls.Model;

namespace SimControls.YokeConnector
{
    public interface IYokeConnection{}
    public class YokeConnection: IYokeConnection
    {
        private readonly DataItem<double> elevatorTrim;

        public YokeConnection(IChYoke yoke, IVariableCache dataStore )
        {
            elevatorTrim =  dataStore.ElevatorTrimPosition();
            yoke.StateChanged += new OnChangeToTrue(() => yoke.VerticalWheelUp, WheelUp).Handler;
            yoke.StateChanged += new OnChangeToTrue(() => yoke.VerticalWheelDown, WheelDown).Handler;
        }

        private void WheelDown() => AdjustElevator(1);
        private const double degreesToRadians = Math.PI / 180.0;

        private void AdjustElevator(int direction) => 
            elevatorTrim.Value = (elevatorTrim.Value + (direction*degreesToRadians / 2.0))
                .Clamp(-20*degreesToRadians, 20*degreesToRadians);

        private void WheelUp() => AdjustElevator(-1);
    }

    public class OnChangeToTrue
    {
        private Func<bool> test;
        private Action effect;

        public OnChangeToTrue(Func<bool> test, Action effect)
        {
            this.test = test;
            this.effect = effect;
        }

        private bool prior = false;
        public void Handler(object? o, EventArgs e)
        {
            var newValue = test();
            if (newValue && !prior) effect();
            prior = newValue;
        }
    }
}
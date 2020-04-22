namespace Icarus.Actuators.Motor
{
    public class SimulatedMotorController : IMotorController
    {
        private double _speedLeft = 0;
        private double _speedRight = 0;

        public void SetLeft(double speed) => _speedLeft = speed;
        public void SetRight(double speed) => _speedRight = speed;
        public double GetLeft() => _speedLeft;
        public double GetRight() => _speedRight;
    }
}

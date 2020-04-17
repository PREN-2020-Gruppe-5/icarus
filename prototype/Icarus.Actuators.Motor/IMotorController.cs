namespace Icarus.Actuators.Motor
{
    public interface IMotorController
    {
        void SetLeft(double speed);
        void SetRight(double speed);
    }
}
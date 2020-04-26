namespace Icarus.Actuators.Motor
{
    public interface IMotorActor
    {
        void SetLeft(double speed);
        void SetRight(double speed);
        double GetLeft();
        double GetRight();
    }
}
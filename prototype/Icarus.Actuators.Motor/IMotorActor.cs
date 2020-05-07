namespace Icarus.Actuators.Motor
{
    public interface IMotorActor
    {
        void SetSpeed(double speed);
        double GetSpeed();
    }
}
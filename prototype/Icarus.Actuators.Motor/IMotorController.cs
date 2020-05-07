using System.Threading.Tasks;

namespace Icarus.Actuators.Motor
{
    public interface IMotorController
    {
        double GetRequestedSpeedDutyCycle();
        void SetForward(MotorSpeed motorSpeed);
        void SetForward(double speedRight, double speedLeft);
        Task TurnLeftAsync();
        void Stop();
    }
}
using Icarus.Actuators.Motor;
using Icarus.Sensors.Tilt;

namespace Icarus.Sensors.Motor
{
    public class SimulatedHallEffectController : IHallEffectController
    {
        // wheel circumference: 20cm
        // 1 rpm = 20cm / minute = 0.33..cm / second = 0.0033.. m/s
        // wheels at 100% with no friction and driving on a flat surface = 150 rpm = 0.5 m/s
        private const double WheelCircumferenceMeters = 0.2;
        private const double DefaultSpeedNoFrictionFlatMetersPerSecond = 0.5;

        private readonly IMotorActor _motorActor;
        private readonly ITiltSensor _tiltSensor;

        public SimulatedHallEffectController(IMotorActor motorActor, ITiltSensor tiltSensor)
        {
            _motorActor = motorActor;
            _tiltSensor = tiltSensor;
        }

        public int GetWheelRpm(WheelLocation wheel)
        {
            var tiltResult = _tiltSensor.GetTilt();
            var rpm = ConvertPwmToRpm(wheel == WheelLocation.Left ? _motorActor.GetLeft() : _motorActor.GetRight());

            // using a simulated formula to calcualte actual RPM for example when climbing an obstacle
            // 45 degrees = only 20% of normal speed. angles over 55 degrees are not climbable and speed is 0
            var actualRpm = (int) (rpm * (1 - (tiltResult.RotationX / 45 * 0.8)));

            // can't go negative speed when driving forward --> friction too high and therefore speed = 0
            return actualRpm > 0 ? actualRpm : 0;
        }

        public static double ConvertPwmToRpm(double pwmDutyCycle) =>
            (DefaultSpeedNoFrictionFlatMetersPerSecond / pwmDutyCycle) / (WheelCircumferenceMeters / 60);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Actuators.Motor
{
    public class MotorSpeedConverter : IMotorSpeedConverter
    {
        public double GetDutyCycleFromSpeed(MotorSpeed motorSpeed)
        {
            switch (motorSpeed)
            {
                case MotorSpeed.Slow:
                    return 0.2;
                case MotorSpeed.Medium:
                    return 0.5;
                case MotorSpeed.Fast:
                    return 0.8;
                case MotorSpeed.Maximum:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(motorSpeed), motorSpeed, null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Actuators.Motor
{
    public interface IMotorSpeedConverter
    {
        double GetDutyCycleFromSpeed(MotorSpeed motorSpeed);
    }
}

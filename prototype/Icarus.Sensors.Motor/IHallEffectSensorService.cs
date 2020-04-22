using System;

namespace Icarus.Sensors.Motor
{
    public interface IHallEffectSensorService
    {
        int GetWheelRpm(WheelLocation wheel);
    }
}

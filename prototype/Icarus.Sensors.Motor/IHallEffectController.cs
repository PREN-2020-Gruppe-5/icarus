using System;

namespace Icarus.Sensors.Motor
{
    public interface IHallEffectController
    {
        int GetWheelRpm(WheelLocation wheel);
    }
}

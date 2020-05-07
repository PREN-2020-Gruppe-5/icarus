using Icarus.Common;
using System.Collections.Generic;
using System.Linq;

namespace Icarus.Sensors.HallEffect
{
    public class HallEffectController : IHallEffectController
    {
        private readonly IDirectional<IHallEffectSensor> halleffectSensors;

        public HallEffectController(IDirectional<IHallEffectSensor> halleffectSensors)
        {
            this.halleffectSensors = halleffectSensors;
        }

        public int GetWheelRpm(WheelLocation wheel)
        {
            if (wheel == WheelLocation.Left)
            {
                var hallEffectSensorResult = halleffectSensors.Left.GetHallEffectSensorResult();
                return (int)(hallEffectSensorResult.DutyCycleA * 1000 * (hallEffectSensorResult.Forward ? 1 : -1));
            }
            else
            {
                var hallEffectSensorResult = halleffectSensors.Right.GetHallEffectSensorResult();
                return (int)(hallEffectSensorResult.DutyCycleA * 1000 * (hallEffectSensorResult.Forward ? 1 : -1));
            }
        }
    }
}

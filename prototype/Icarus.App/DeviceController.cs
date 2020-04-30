using Icarus.Actuators.Motor;
using Icarus.Sensors.HallEffect;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;

namespace Icarus.App
{
    public class DeviceController
    {
        private readonly IMotorController motorController;
        private readonly IHallEffectController hallEffectController;
        private readonly IObjectDetectionController objectDetectionController;
        private readonly ITiltController tiltController;
        private readonly ITofController tofController;

        public DeviceController(IMotorController motorController, IHallEffectController hallEffectController, IObjectDetectionController objectDetectionController, ITiltController tiltController, ITofController tofController)
        {
            this.motorController = motorController;
            this.hallEffectController = hallEffectController;
            this.objectDetectionController = objectDetectionController;
            this.tiltController = tiltController;
            this.tofController = tofController;
        }
    }
}

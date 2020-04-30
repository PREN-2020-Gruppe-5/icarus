using Icarus.Actuators.Motor;
using Icarus.Sensors.HallEffect;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.App
{
    public class AppModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            MotorModule.Initialize(serviceCollection);
            HallEffectModule.Initialize(serviceCollection);
            ObjectDetectionModule.Initialize(serviceCollection);
            TiltModule.Initialize(serviceCollection);
            TofModule.Initialize(serviceCollection);

            serviceCollection.AddSingleton<DeviceController>();
        }
    }
}

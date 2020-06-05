using Icarus.Actuators.Motor;
using Icarus.App;
using Icarus.Sensors.HallEffect;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Scenarios
{
    public class SimulationModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            MotorSimulationModule.Initialize(serviceCollection);
            HallEffectModule.Initialize(serviceCollection);
            ObjectDetectionSimulationModule.Initialize(serviceCollection);
            TiltSimulationModule.Initialize(serviceCollection);
            TofSimulationModule.Initialize(serviceCollection);

            serviceCollection.AddSingleton<DeviceController>();
        }
    }
}

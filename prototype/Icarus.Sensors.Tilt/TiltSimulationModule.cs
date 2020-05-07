using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tilt
{
    public class TiltSimulationModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITiltSensor, TiltSensorSimulator>();
            serviceCollection.AddSingleton<ITiltController, TiltController>();
        }
    }
}

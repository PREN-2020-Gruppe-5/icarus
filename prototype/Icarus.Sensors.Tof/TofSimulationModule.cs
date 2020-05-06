using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tof
{
    public class TofSimulationModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITofSensor, TofSensorSimulator>();
            serviceCollection.AddSingleton<ITofController, TofController>();
        }
    }
}

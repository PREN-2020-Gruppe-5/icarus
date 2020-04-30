using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tof
{
    public class TofModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITofSensor, TofSensor>();
            serviceCollection.AddSingleton<ITofController, TofController>();
        }
    }
}

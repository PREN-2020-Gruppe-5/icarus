using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tilt
{
    public class TiltModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITiltSensor, TiltSensor>();
            serviceCollection.AddSingleton<ITiltController, TiltController>();
        }
    }
}

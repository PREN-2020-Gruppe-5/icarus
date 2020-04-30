using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.HallEffect
{
    public class HallEffectModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHallEffectSensor, HallEffectSensor>();
            serviceCollection.AddSingleton<IHallEffectController, HallEffectController>();
        }
    }
}

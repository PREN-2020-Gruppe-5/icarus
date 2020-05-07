using Icarus.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.HallEffect
{
    public class HallEffectModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDirectional<IHallEffectSensor>>(new Directional<IHallEffectSensor>(new HallEffectSensor(), new HallEffectSensor()));
            serviceCollection.AddSingleton<IHallEffectController, HallEffectController>();
        }
    }
}

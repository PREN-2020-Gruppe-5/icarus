using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.Pwm;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Actuators.Motor
{
    public class MotorModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            var gpio = new GpioController(PinNumberingScheme.Logical, new SysFsDriver());

            // left
            var right = PwmChannel.Create(0, 2, 20000, 0.2);

            // right
            var left = PwmChannel.Create(0, 0, 20000, 0.2);

            serviceCollection.AddSingleton<IMotorActor>(p => new MotorActor(left, right, gpio));
            serviceCollection.AddSingleton(gpio);
            serviceCollection.AddSingleton<IMotorController, MotorController>();
        }
    }
}

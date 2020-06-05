using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.Pwm;
using Icarus.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Icarus.Actuators.Motor
{
    public class MotorSimulationModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            // use TryAddSingleton since other Controllers may have already registered this singleton
            serviceCollection.TryAddSingleton(new GpioController(PinNumberingScheme.Logical, new SysFsDriver()));

            serviceCollection.AddSingleton<IDirectional<PwmChannel>>(p =>
            {
                var left = PwmChannel.Create(0, 0, 20000, 0.2);
                var right = PwmChannel.Create(0, 2, 20000, 0.2);
                return new Directional<PwmChannel>(left, right);
            });

            serviceCollection.AddSingleton<IDirectional<IMotorActor>>(p =>
            {
                var directionalPwms = p.GetService<IDirectional<PwmChannel>>();
                var gpio = p.GetService<GpioController>();
                var left = new MotorActor(directionalPwms.Left, gpio, 77, 78);
                var right = new MotorActor(directionalPwms.Right, gpio, 79, 80);
                return new Directional<IMotorActor>(left, right);
            });

            serviceCollection.AddSingleton<IMotorController, MotorControllerSimulator>();
        }
    }
}

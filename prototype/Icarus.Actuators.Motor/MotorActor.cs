using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.Pwm;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Actuators.Motor
{
    public class MotorActor : IMotorActor, IDisposable
    {
        private readonly PwmChannel _left;
        private readonly PwmChannel _right;
        private readonly GpioController _gpio;

        private MotorActor(PwmChannel left, PwmChannel right, GpioController gpio)
        {
            _left = left;
            _right = right;
            _gpio = gpio;

            _gpio.OpenPin(77, PinMode.Output);        // M1INA motor 1 direction A
            _gpio.OpenPin(78, PinMode.Output);        // M1INB motor 1 direction B

            _gpio.OpenPin(79, PinMode.Output);        // M1INA motor 2 direction A
            _gpio.OpenPin(80, PinMode.Output);        // M1INB motor 2 direction B

            // initial state forward
            _gpio.Write(77, PinValue.High);
            _gpio.Write(78, PinValue.Low);

            _gpio.Write(79, PinValue.High);
            _gpio.Write(78, PinValue.Low);

            _left.Start();
            _right.Start();
        }

        public void SetLeft(double speed)
        {
            SetMotorDirectionLeft(speed < 0 ? MotorDirection.Backward : MotorDirection.Forward);
            _left.DutyCycle = Math.Abs(speed);
        }

        public double GetLeft()
        {
            return _left.DutyCycle;
        }

        public void SetRight(double speed)
        { 
            SetMotorDirectionRight(speed < 0 ? MotorDirection.Backward : MotorDirection.Forward);
            _right.DutyCycle = Math.Abs(speed);
        }

        public double GetRight()
        {
            return _right.DutyCycle;
        }
        
        private void SetMotorDirectionLeft(MotorDirection direction)
        {
            _gpio.Write(77, direction == MotorDirection.Forward ? PinValue.High : PinValue.Low);
            _gpio.Write(78, direction == MotorDirection.Forward ? PinValue.Low : PinValue.High);
        }

        private void SetMotorDirectionRight(MotorDirection direction)
        {
            _gpio.Write(79, direction == MotorDirection.Forward ? PinValue.High : PinValue.Low);
            _gpio.Write(80, direction == MotorDirection.Forward ? PinValue.Low : PinValue.High);
        }

        public static void Initialize(IServiceCollection serviceCollection)
        {
            var gpio = new GpioController(PinNumberingScheme.Logical, new SysFsDriver());
            
            // left
            var right = PwmChannel.Create(0, 2, 20000, 0.2);

            // right
            var left = PwmChannel.Create(0, 0, 20000, 0.2);

            serviceCollection.AddSingleton<IMotorActor>(p => new MotorActor(left, right, gpio));
            serviceCollection.AddSingleton(gpio);
        }

        public void Dispose()
        {
            _gpio?.ClosePin(77);
            _gpio?.ClosePin(78);
            _gpio?.ClosePin(79);
            _gpio?.ClosePin(80);
            _left?.Dispose();
            _right?.Dispose();
            _gpio?.Dispose();
        }
    }
}

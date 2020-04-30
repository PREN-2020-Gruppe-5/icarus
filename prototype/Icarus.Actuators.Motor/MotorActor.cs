using System;
using System.Device.Gpio;
using System.Device.Pwm;

namespace Icarus.Actuators.Motor
{
    public class MotorActor : IMotorActor, IDisposable
    {
        private readonly PwmChannel left;
        private readonly PwmChannel right;
        private readonly GpioController gpio;

        public MotorActor(PwmChannel left, PwmChannel right, GpioController gpio)
        {
            this.left = left;
            this.right = right;
            this.gpio = gpio;

            this.gpio.OpenPin(77, PinMode.Output);        // M1INA motor 1 direction A
            this.gpio.OpenPin(78, PinMode.Output);        // M1INB motor 1 direction B

            this.gpio.OpenPin(79, PinMode.Output);        // M1INA motor 2 direction A
            this.gpio.OpenPin(80, PinMode.Output);        // M1INB motor 2 direction B

            // initial state forward
            this.gpio.Write(77, PinValue.High);
            this.gpio.Write(78, PinValue.Low);

            this.gpio.Write(79, PinValue.High);
            this.gpio.Write(78, PinValue.Low);

            this.left.Start();
            this.right.Start();
        }

        public void SetLeft(double speed)
        {
            SetMotorDirectionLeft(speed < 0 ? MotorDirection.Backward : MotorDirection.Forward);
            this.left.DutyCycle = Math.Abs(speed);
        }

        public double GetLeft()
        {
            return this.left.DutyCycle;
        }

        public void SetRight(double speed)
        { 
            SetMotorDirectionRight(speed < 0 ? MotorDirection.Backward : MotorDirection.Forward);
            this.right.DutyCycle = Math.Abs(speed);
        }

        public double GetRight()
        {
            return this.right.DutyCycle;
        }
        
        private void SetMotorDirectionLeft(MotorDirection direction)
        {
            this.gpio.Write(77, direction == MotorDirection.Forward ? PinValue.High : PinValue.Low);
            this.gpio.Write(78, direction == MotorDirection.Forward ? PinValue.Low : PinValue.High);
        }

        private void SetMotorDirectionRight(MotorDirection direction)
        {
            this.gpio.Write(79, direction == MotorDirection.Forward ? PinValue.High : PinValue.Low);
            this.gpio.Write(80, direction == MotorDirection.Forward ? PinValue.Low : PinValue.High);
        }

        public void Dispose()
        {
            this.gpio?.ClosePin(77);
            this.gpio?.ClosePin(78);
            this.gpio?.ClosePin(79);
            this.gpio?.ClosePin(80);
            this.left?.Dispose();
            this.right?.Dispose();
            this.gpio?.Dispose();
        }
    }
}

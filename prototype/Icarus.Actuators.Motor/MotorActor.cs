using System;
using System.Device.Gpio;
using System.Device.Pwm;

namespace Icarus.Actuators.Motor
{
    public class MotorActor : IMotorActor, IDisposable
    {
        private readonly PwmChannel pwnChannel;
        private readonly GpioController gpio;
        private readonly int inaPin;
        private readonly int inbPin;

        public MotorActor(PwmChannel pwnChannel, GpioController gpio, int inaPin, int inbPin)
        {
            this.pwnChannel = pwnChannel;
            this.gpio = gpio;
            this.inaPin = inaPin;
            this.inbPin = inbPin;

            this.gpio.OpenPin(inaPin, PinMode.Output);        // M1INA motor 1 direction A
            this.gpio.OpenPin(inbPin, PinMode.Output);        // M1INB motor 1 direction B

            // initial state forward
            this.gpio.Write(inaPin, PinValue.High);
            this.gpio.Write(inbPin, PinValue.Low);

            this.pwnChannel.Start();
        }

        public void SetSpeed(double speed)
        {
            SetMotorDirection(speed < 0 ? MotorDirection.Backward : MotorDirection.Forward);
            this.pwnChannel.DutyCycle = Math.Abs(speed);
        }

        public double GetSpeed()
        {
            return this.pwnChannel.DutyCycle;
        }

        private void SetMotorDirection(MotorDirection direction)
        {
            this.gpio.Write(this.inaPin, direction == MotorDirection.Forward ? PinValue.High : PinValue.Low);
            this.gpio.Write(this.inbPin, direction == MotorDirection.Forward ? PinValue.Low : PinValue.High);
        }

        public void Dispose()
        {
            this.gpio?.ClosePin(inaPin);
            this.gpio?.ClosePin(inbPin);
            this.pwnChannel?.Dispose();
            this.gpio?.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Text;

namespace Icarus.Sensors.Button
{
    public class ButtonSensor : IButtonSensor, IDisposable
    {
        private readonly GpioController gpioController;
        private const int ButtonPin = 8;

        public ButtonSensor()
        {
            this.gpioController = new GpioController(PinNumberingScheme.Logical, new SysFsDriver());
            this.gpioController.OpenPin(ButtonPin, PinMode.InputPullUp);
        }

        public bool GetButtonPressed()
        {
            return this.gpioController.Read(ButtonPin) == PinValue.Low;
        }

        public void Dispose()
        {
            this.gpioController.ClosePin(ButtonPin);
        }
    }
}

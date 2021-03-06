﻿using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;

namespace Icarus.Sensors.HallEffect
{
    public class HallEffectSensor : IHallEffectSensor
    {
        private PinEventTypes previousPinEventType;

        public HallEffectSensor()
        {
            var gpioController = new GpioController(PinNumberingScheme.Logical, new SysFsDriver());
            gpioController.RegisterCallbackForPinValueChangedEvent(4, PinEventTypes.Falling | PinEventTypes.Rising, this.PinValueChanged);
            gpioController.WaitForEvent(3, PinEventTypes.Falling, CancellationToken.None);
        }

        private void PinValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            this.previousPinEventType = pinValueChangedEventArgs.ChangeType;
        }

        public HallEffectSensorResult GetHallEffectSensorResult()
        {
            var random = new Random();

            return new HallEffectSensorResult()
            {
                DutyCycleA = random.NextDouble(),
                DutyCycleB = random.NextDouble(),
                DutyCycleC = random.NextDouble(),
                Forward = random.NextDouble() > 0.5
            };
        }
    }
}

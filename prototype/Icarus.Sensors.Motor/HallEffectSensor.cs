using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;

namespace Icarus.Sensors.Motor
{
    public class HallEffectSensor : IHallEffectSensor
    {
        private PinEventTypes _previousPinEventType;

        public HallEffectSensor()
        {
            var gpioController = new GpioController(PinNumberingScheme.Logical, new SysFsDriver());
            gpioController.RegisterCallbackForPinValueChangedEvent(4, PinEventTypes.Falling | PinEventTypes.Rising, PinValueChanged);
            gpioController.WaitForEvent(3, PinEventTypes.Falling, CancellationToken.None);
        }

        private void PinValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            _previousPinEventType = pinValueChangedEventArgs.ChangeType;
        }

        public HallEffectSensorResult GetHallEffectSensorResult()
        {
            var random = new Random();

            return new HallEffectSensorResult()
            {
                DutyCycleA = random.NextDouble(),
                DutyCycleB = random.NextDouble(),
                DutyCycleC = random.NextDouble(),
                FrequencyA = random.Next(15000),
                FrequencyB = random.Next(15000),
                FrequencyC = random.Next(15000)
            };
        }
    }
}

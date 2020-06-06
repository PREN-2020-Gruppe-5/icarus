using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Sensors.Button
{
    public class ButtonController : IButtonController
    {
        private readonly IButtonSensor buttonSensor;

        public ButtonController(IButtonSensor buttonSensor)
        {
            this.buttonSensor = buttonSensor;
        }

        public bool GetButtonPressed() => this.buttonSensor.GetButtonPressed();
    }
}

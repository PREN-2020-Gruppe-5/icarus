using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Sensors.Button
{
    public interface IButtonController
    {
        bool GetButtonPressed();
    }
}

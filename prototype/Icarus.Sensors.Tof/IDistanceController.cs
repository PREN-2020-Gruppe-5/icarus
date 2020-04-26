using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Sensors.Tof
{
    public interface IDistanceController
    {
        double GetDistanceMillimeters();
    }
}

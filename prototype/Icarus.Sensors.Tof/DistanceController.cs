using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Sensors.Tof
{
    public class DistanceController : IDistanceSensor
    {
        private readonly IDistanceSensor _distanceSensor;

        public DistanceController(IDistanceSensor distanceSensor)
        {
            _distanceSensor = distanceSensor;
        }

        public double GetDistanceMillimeters() => _distanceSensor.GetDistanceMillimeters();
    }
}

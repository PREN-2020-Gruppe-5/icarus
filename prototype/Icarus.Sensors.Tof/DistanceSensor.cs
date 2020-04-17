using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tof
{

    public class DistanceSensor
    {
        private readonly I2cDevice _tof;
        private readonly VL53L1X _vl;

        private DistanceSensor()
        {
            _tof = I2cDevice.Create(new I2cConnectionSettings(0x1, 0x29));
            _vl = new VL53L1X(_tof);

            _vl.init(false);
            _vl.startContinuous(100);

        }

        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(new DistanceSensor());
        }

        public double GetDistance()
        {
            return _vl.GetDistanceMillimeters();
        }
    }
}

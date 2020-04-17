﻿using System.Device.I2c;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.Tof
{
    public class DistanceSensor : IDistanceSensor
    {
        private readonly VL53L1X _vl;

        private DistanceSensor()
        {
            var tof = I2cDevice.Create(new I2cConnectionSettings(0x1, 0x29));
            _vl = new VL53L1X(tof);

            _vl.init(false);
            _vl.startContinuous(100);

        }

        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDistanceSensor>(new DistanceSensor());
        }

        public double GetDistance()
        {
            return _vl.GetDistanceMillimeters();
        }
    }
}

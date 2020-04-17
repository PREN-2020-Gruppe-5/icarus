using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Sensors.Tilt
{
    public class ConfigurableTiltSensor : ITiltSensor
    {
        private readonly ITiltConfiguration _tiltConfiguration;

        public ConfigurableTiltSensor(ITiltConfiguration tiltConfiguration)
        {
            _tiltConfiguration = tiltConfiguration;
        }

        public TiltResult GetTilt()
        {
            return new TiltResult
            {
                RotationX = _tiltConfiguration.X,
                RotationY = _tiltConfiguration.Y
            };
        }
    }
}

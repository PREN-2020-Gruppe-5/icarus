using System;

namespace Icarus.Sensors.Tilt
{
    public class RandomTiltSensor : ITiltSensor
    {
        private readonly int _minDegreesX;
        private readonly int _maxDegreesX;
        private readonly int _minDegreesY;
        private readonly int _maxDegreesY;

        public RandomTiltSensor(int minDegreesX = -60, int maxDegreesX = 60, int minDegreesY = -60, int maxDegreesY = 60)
        {
            _minDegreesX = minDegreesX;
            _maxDegreesX = maxDegreesX;
            _minDegreesY = minDegreesY;
            _maxDegreesY = maxDegreesY;
        }

        private readonly Random _random = new Random();

        public TiltResult GetTilt()
        {
            return new TiltResult
            {
                // ranging from -60 to 60 degrees where 0 degress is flat. 45 deg is going uphill and -45 deg is going downhill
                RotationX = _random.Next(_minDegreesX, _maxDegreesX),
                // same as X but -45 means right side is higher than left side. +45 means left side is higher than right side
                RotationY = _random.Next(_minDegreesY, _maxDegreesY)
            };
        }
    }
}

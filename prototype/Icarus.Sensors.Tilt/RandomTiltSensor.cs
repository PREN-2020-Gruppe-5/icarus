using System;

namespace Icarus.Sensors.Tilt
{
    public class RandomTiltSensor : ITiltSensor
    {
        private readonly int minDegreesX;
        private readonly int maxDegreesX;
        private readonly int minDegreesY;
        private readonly int maxDegreesY;

        public RandomTiltSensor(int minDegreesX = -60, int maxDegreesX = 60, int minDegreesY = -60, int maxDegreesY = 60)
        {
            this.minDegreesX = minDegreesX;
            this.maxDegreesX = maxDegreesX;
            this.minDegreesY = minDegreesY;
            this.maxDegreesY = maxDegreesY;
        }

        private readonly Random random = new Random();

        public TiltResult GetTilt()
        {
            return new TiltResult
            {
                // ranging from -60 to 60 degrees where 0 degress is flat. 45 deg is going uphill and -45 deg is going downhill
                RotationX = this.random.Next(this.minDegreesX, this.maxDegreesX),
                // same as X but -45 means right side is higher than left side. +45 means left side is higher than right side
                RotationY = this.random.Next(this.minDegreesY, this.maxDegreesY)
            };
        }
    }
}

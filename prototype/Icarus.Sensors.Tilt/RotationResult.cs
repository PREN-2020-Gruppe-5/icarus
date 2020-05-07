namespace Icarus.Sensors.Tilt
{
    public class RotationResult
    {
        public double GyroscopeX { get; set; }
        public double GyroscopeY { get; set; }
        public double GyroscopeZ { get; set; }

        public double AccelerationX { get; set; }
        public double AccelerationY { get; set; }
        public double AccelerationZ { get; set; }

        public double RotationX { get; set; }
        public double RotationY { get; set; }
    }
}
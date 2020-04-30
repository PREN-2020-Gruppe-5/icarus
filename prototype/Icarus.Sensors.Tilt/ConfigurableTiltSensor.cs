namespace Icarus.Sensors.Tilt
{
    public class ConfigurableTiltSensor : ITiltSensor
    {
        private readonly ITiltConfiguration tiltConfiguration;

        public ConfigurableTiltSensor(ITiltConfiguration tiltConfiguration)
        {
            this.tiltConfiguration = tiltConfiguration;
        }

        public TiltResult GetTilt()
        {
            return new TiltResult
            {
                RotationX = this.tiltConfiguration.X,
                RotationY = this.tiltConfiguration.Y
            };
        }
    }
}

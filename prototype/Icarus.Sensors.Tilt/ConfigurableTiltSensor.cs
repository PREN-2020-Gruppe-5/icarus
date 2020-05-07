namespace Icarus.Sensors.Tilt
{
    public class ConfigurableTiltSensor : ITiltSensor
    {
        private readonly ITiltConfiguration tiltConfiguration;

        public ConfigurableTiltSensor(ITiltConfiguration tiltConfiguration)
        {
            this.tiltConfiguration = tiltConfiguration;
        }

        public RotationResult GetTilt()
        {
            return new RotationResult
            {
                RotationX = this.tiltConfiguration.X,
                RotationY = this.tiltConfiguration.Y
            };
        }
    }
}

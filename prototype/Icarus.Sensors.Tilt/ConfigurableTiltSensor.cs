namespace Icarus.Sensors.Tilt
{
    public class ConfigurableTiltSensor : ITiltSensor
    {
        private readonly ITiltConfiguration tiltConfiguration;

        public ConfigurableTiltSensor(ITiltConfiguration tiltConfiguration)
        {
            this.tiltConfiguration = tiltConfiguration;
        }

        public RotationResult GetRotationResult()
        {
            return new RotationResult
            {
                XRotation = this.tiltConfiguration.X,
                YRotation = this.tiltConfiguration.Y
            };
        }
    }
}

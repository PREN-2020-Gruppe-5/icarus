namespace Icarus.Sensors.Tilt
{
    public class TiltSensorSimulator : ITiltSensor
    {
        private RotationResult rotationResult;

        public RotationResult GetRotationResult()
        {
            return this.rotationResult;
        }

        public void SetRotationResult(double xRotation, double yRotation)
        {
            this.rotationResult = new RotationResult
            {
                XRotation = xRotation,
                YRotation = yRotation
            };
        }
    }
}

namespace Icarus.Sensors.Tilt
{
    public class TiltResult
    {
        public TiltResult(OrientationInformation orientationInformation)
        {
            this.OrientationInformation = orientationInformation;
        }

        public OrientationInformation OrientationInformation { get; }
    }
}

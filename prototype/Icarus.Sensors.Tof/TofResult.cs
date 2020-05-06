namespace Icarus.Sensors.Tof
{
    public class TofResult
    {
        public TofResult(double distanceInMillimeters, DistanceInformation distanceInformation)
        {
            this.DistanceInMillimeters = distanceInMillimeters;
            this.DistanceInformation = distanceInformation;
        }

        public double DistanceInMillimeters { get; }
        public DistanceInformation DistanceInformation { get; }
    }
}

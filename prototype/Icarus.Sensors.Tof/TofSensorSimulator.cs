namespace Icarus.Sensors.Tof
{
    public class TofSensorSimulator : ITofSensor
    {
        private double distanceInMillimeters;

        public double GetDistanceInMillimeters()
        {
            return this.distanceInMillimeters;
        }

        public void SetDistanceInMillimeters(double simulatedDistanceInMillimeters)
        {
            this.distanceInMillimeters = simulatedDistanceInMillimeters;
        }
    }
}

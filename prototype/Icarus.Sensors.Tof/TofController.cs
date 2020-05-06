namespace Icarus.Sensors.Tof
{
    public class TofController : ITofController
    {
        private const double NegativeLimit = 0;
        private const double NearMissLimit = 100;
        private const double VoidLimit = 500;

        private readonly ITofSensor tofSensor;

        public TofController(ITofSensor tofSensor)
        {
            this.tofSensor = tofSensor;
        }

        public TofResult GetTofResult()
        {
            var distanceInMillimeters = this.tofSensor.GetDistanceInMillimeters();
            var distanceInformation = this.MapDistanceInMillimetersToDistanceInformation(distanceInMillimeters);

            return new TofResult(distanceInMillimeters, distanceInformation);
        }

        private DistanceInformation MapDistanceInMillimetersToDistanceInformation(double distanceInMillimeters)
        {
            DistanceInformation result;

            if (distanceInMillimeters < NegativeLimit)
            {
                result = DistanceInformation.Negative;
            }
            else if (distanceInMillimeters >= NegativeLimit && distanceInMillimeters < NearMissLimit)
            {
                result = DistanceInformation.NearMiss;
            }
            else if (distanceInMillimeters >= NearMissLimit && distanceInMillimeters < VoidLimit)
            {
                result = DistanceInformation.TrafficConeDetected;
            }
            else
            {
                result = DistanceInformation.Void;
            }

            return result;
        }
    }
}

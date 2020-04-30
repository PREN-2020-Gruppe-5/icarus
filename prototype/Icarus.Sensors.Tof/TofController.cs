namespace Icarus.Sensors.Tof
{
    public class TofController : ITofController
    {
        private readonly ITofSensor tofSensor;

        public TofController(ITofSensor tofSensor)
        {
            this.tofSensor = tofSensor;
        }

        public double GetDistanceMillimeters() => this.tofSensor.GetDistanceMillimeters();
    }
}

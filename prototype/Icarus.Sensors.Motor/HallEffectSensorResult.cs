namespace Icarus.Sensors.Motor
{
    public class HallEffectSensorResult
    {
        public int FrequencyA { get; set; }
        public int FrequencyB { get; set; }
        public int FrequencyC { get; set; }

        public double DutyCycleA { get; set; }
        public double DutyCycleB { get; set; }
        public double DutyCycleC { get; set; }
    }
}
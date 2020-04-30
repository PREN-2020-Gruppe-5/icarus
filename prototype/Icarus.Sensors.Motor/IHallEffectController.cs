namespace Icarus.Sensors.HallEffect
{
    public interface IHallEffectController
    {
        int GetWheelRpm(WheelLocation wheel);
    }
}

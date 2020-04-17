using System;
using System.Threading.Tasks;

namespace Icarus.Sensors.Tilt
{
    public interface ITiltConfiguration
    {
        double X { get; }

        double Y { get; }

        void SetTilt((double X, double Y) vehicleOrientation);
        Task SetTilt((double X, double Y) vehicleOrientation, TimeSpan timeToReachVehicleOrientation);
    }
}
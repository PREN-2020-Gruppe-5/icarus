using System;
using System.Threading.Tasks;

namespace Icarus.Sensors.Tilt
{
    public class TiltConfiguration : ITiltConfiguration
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public void SetTilt((double X, double Y) vehicleOrientation)
        {
            X = vehicleOrientation.X;
            Y = vehicleOrientation.Y;
        }

        public async Task SetTilt((double X, double Y) vehicleOrientation, TimeSpan timeToReachVehicleOrientation)
        {
            var startingOrientation = (X, Y);

            var degreesXPerTick = (vehicleOrientation.X - startingOrientation.X) / timeToReachVehicleOrientation.TotalMilliseconds;
            var degreesYPerTick = (vehicleOrientation.Y - startingOrientation.Y) / timeToReachVehicleOrientation.TotalMilliseconds;
            for (var i = 0; i < timeToReachVehicleOrientation.TotalMilliseconds / 10; i++)
            {
                SetTilt(VehicleOrientation.Custom(X + degreesXPerTick * 10, Y + degreesYPerTick * 10));
                await Task.Delay(10);
            }

            SetTilt(vehicleOrientation);
        }
    }
}
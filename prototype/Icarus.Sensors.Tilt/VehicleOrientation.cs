using System;

namespace Icarus.Sensors.Tilt
{
    public static class VehicleOrientation
    {
        public static (int x, int y) Flat => (0, 0);
        public static (int x, int y) ClimbingUpObstacle => (45, 0);
        public static (int x, int y) ClimbingDownObstacle => (-45, 0);
        public static (double x, double y) Custom(double x, double y) => (Math.Round(x, 2), Math.Round(y, 2));
    }
}
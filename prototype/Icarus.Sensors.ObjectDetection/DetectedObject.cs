using System.Drawing;

namespace Icarus.Sensors.ObjectDetection
{
    public class DetectedObject
    {
        public string Name { get; set; }

        public double Confidence { get; set; }

        public Rectangle Location { get; set; }
    }
}
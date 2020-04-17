using System.Drawing;
using System.Collections.Generic;

namespace Icarus.Common.Contracts.Notifications
{
    public class ObjectsDetectedMessage
    {
        public List<DetectedObject> Objects { get; set; }
    }

    public class DetectedObject
    {
        public string Name { get; set; }

        public double Confidence { get; set; }

        public Rectangle Location { get; set; }
    }
}

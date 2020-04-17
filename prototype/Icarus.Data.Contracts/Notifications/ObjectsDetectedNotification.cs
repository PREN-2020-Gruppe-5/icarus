using System;
using System.Collections.Generic;
using System.Drawing;

namespace Icarus.Data.Contracts.Notifications
{
    public class DetectedObject
    {
        public string Name { get; set; }

        public double Confidence { get; set; }

        public Rectangle Location { get; set; }
    }

    public class ObjectsDetectedNotification
    {
        public List<DetectedObject> Objects { get; set; }
    }
}

using System.Linq;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetectionController : IObjectDetectionController
    {
        private const string TrafficConeName = "Traffic_Cone";
        private const double ConfidenceLimit = 0.6;

        private readonly IObjectDetectionSensor objectDetectionSensor;

        public ObjectDetectionController(IObjectDetectionSensor objectDetectionSensor)
        {
            this.objectDetectionSensor = objectDetectionSensor;
        }

        public DetectedObject GetNearestDetectedTrafficCone()
        {
            var detectedObjects = this.objectDetectionSensor.GetDetectedObjectsFromCamera();
            var detectedTrafficCones = detectedObjects.Where(_ => _.Name == TrafficConeName && _.Confidence >= ConfidenceLimit);
            return detectedTrafficCones.OrderByDescending(_ => _.Location.Width * _.Location.Height).FirstOrDefault();
        }
    }
}
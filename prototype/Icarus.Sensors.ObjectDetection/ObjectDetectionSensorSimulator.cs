using System.Collections.Generic;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetectionSensorSimulator : IObjectDetectionSensor
    {
        private List<DetectedObject> detectedObjects = new List<DetectedObject>();

        public Task RunDetectionFromCamera()
        {
            return Task.Delay(0);
        }

        public Task RunDetectionFromVideo(string videoFileName)
        {
            return Task.Delay(0);
        }

        public Task RunDetectionFromImage(string imageFileName)
        {
            return Task.Delay(0);
        }

        public List<DetectedObject> GetDetectedObjects()
        {
            return this.detectedObjects;
        }

        public void SetDetectedObjects(List<DetectedObject> simulatedDetectedObjects)
        {
            this.detectedObjects = simulatedDetectedObjects;
        }
    }
}

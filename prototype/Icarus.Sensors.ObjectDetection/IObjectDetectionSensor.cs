using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public interface IObjectDetectionSensor
    {
        List<DetectedObject> GetDetectedObjectsFromCamera();
        List<DetectedObject> GetDetectedObjectsFromVideo(string videoFileName);
        List<DetectedObject> GetDetectedObjectsFromImage(string imageFileName);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public interface IObjectDetectionSensor
    {
        Task RunDetectionFromCamera();
        Task RunDetectionFromVideo(string videoFileName);
        Task RunDetectionFromImage(string imageFileName);

        List<DetectedObject> GetDetectedObjects();
    }
}
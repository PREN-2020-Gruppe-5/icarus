using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public interface IObjectDetectionSensor
    {
        void SetCallback(Action<List<DetectedObject>> callback);
        Task RunVideoDetectionAsync(string videoFileName, CancellationToken cancellationToken);
        Task RunPictureDetectionAsync(string pictureFileName, CancellationToken cancellationToken);
    }
}
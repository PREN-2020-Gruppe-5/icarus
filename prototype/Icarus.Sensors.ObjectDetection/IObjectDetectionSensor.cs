﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public interface IObjectDetectionSensor
    {
        void SetCallback(Action<List<DetectedObject>> callback);
        Task RunDetectionAsync(string videoFileName, CancellationToken cancellationToken);
    }
}
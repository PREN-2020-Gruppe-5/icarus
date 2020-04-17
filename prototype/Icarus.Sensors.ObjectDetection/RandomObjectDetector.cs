using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public class RandomObjectDetector : IObjectDetector
    {
        private Action<List<DetectedObject>> _detectedObjectCallback;

        public void SetCallback(Action<List<DetectedObject>> callback)
        {
            _detectedObjectCallback = callback;
        }

        public async Task RunDetectionAsync(string videoFileName, CancellationToken cancellationToken)
        {
            var random = new Random();

            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (random.NextDouble() < 0.3)
                    {
                        continue;
                    }

                    var objectsFound = random.Next(1, 5);
                    var detectedObjects = new List<DetectedObject>();

                    for (var i = 0; i < objectsFound; i++)
                    {
                        detectedObjects.Add(new DetectedObject
                        {
                            Location = new Rectangle(random.Next(0, 200), random.Next(0, 200), random.Next(0, 200), random.Next(0, 200)),
                            Confidence = random.NextDouble(),
                            Name = "trafficcone"
                        });
                    }

                    _detectedObjectCallback?.Invoke(detectedObjects.ToList());

                    await Task.Delay(100, cancellationToken);
                }
            }, cancellationToken);
        }
    }
}
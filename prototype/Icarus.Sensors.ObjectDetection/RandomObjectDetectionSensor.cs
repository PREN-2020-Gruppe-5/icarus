using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Icarus.Sensors.ObjectDetection
{
    public class RandomObjectDetectionSensor : IObjectDetectionSensor
    {
        private Action<List<DetectedObject>> detectedObjectCallback;
        private readonly Random random = new Random();

        public void SetCallback(Action<List<DetectedObject>> callback)
        {
            this.detectedObjectCallback = callback;
        }

        public async Task RunVideoDetectionAsync(string videoFileName, CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    RunSingleDetection();
                    await Task.Delay(100, cancellationToken);
                }
            }, cancellationToken);
        }

        public Task RunPictureDetectionAsync(string pictureFileName, CancellationToken cancellationToken)
        {
            RunSingleDetection();
            return Task.CompletedTask;
        }

        private void RunSingleDetection()
        {
            if (random.NextDouble() < 0.3)
            {
                return;
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

            this.detectedObjectCallback?.Invoke(detectedObjects.ToList());
        }

        public List<DetectedObject> GetDetectedObjectsFromCamera()
        {
            throw new NotImplementedException();
        }

        public List<DetectedObject> GetDetectedObjectsFromVideo(string videoFileName)
        {
            throw new NotImplementedException();
        }

        public List<DetectedObject> GetDetectedObjectsFromImage(string imageFileName)
        {
            throw new NotImplementedException();
        }
    }
}
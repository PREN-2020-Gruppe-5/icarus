using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetector : IObjectDetector
    {
        private const string DefaultVideoFileName = "traffic_cones.mp4";
        private Action<List<DetectedObject>> _detectedObjectCallback;

        public void SetCallback(Action<List<DetectedObject>> callback)
        {
            _detectedObjectCallback = callback;
        }

        public async Task RunDetectionAsync(string videoFileName = DefaultVideoFileName, CancellationToken cancellationToken = default)
        {
            var arguments = $"detector demo data/obj.data cfg/yolov3-tiny-traffic_cone.cfg yolov3-tiny-obj_final.weights -dont_show -ext_output {videoFileName}";
            Console.WriteLine($"Starting ObjectDetection with arguments '{arguments}'");
            var darknetYolo = Cli.Wrap("/app/darknet/darknet").WithArguments(arguments).WithWorkingDirectory("/app/darknet");

            var detectedObjects = new List<DetectedObject>();

            await foreach (var cmdEvent in darknetYolo.ListenAsync(cancellationToken))
            {
                if (!(cmdEvent is StandardOutputCommandEvent stdOut))
                {
                    continue;
                }

                if (stdOut.Text.Contains("trafficcone"))
                {
                    var numbers = Regex.Matches(stdOut.Text, "[0-9]{1,4}").Select(p => Convert.ToInt32(p.Value)).ToArray();

                    var confidence = (double)numbers[0] / 100;
                    var x = numbers[1];
                    var y = numbers[2];
                    var width = numbers[3];
                    var height = numbers[4];

                    detectedObjects.Add(new DetectedObject { Name = "trafficcone", Confidence = confidence, Location = new Rectangle(x, y, width, height) });

                }
                else if (detectedObjects.Any())
                {
                    _detectedObjectCallback?.Invoke(detectedObjects);
                    detectedObjects.Clear();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetectionSensor : IObjectDetectionSensor
    {
        private const string DefaultVideoFileName = "traffic_cones.mp4";
        private const string DataFilePath = "data/obj.data";
        private const string CfgFilePath = "cfg/yolov3-tiny-traffic_cone.cfg";
        private const string WeightsFilePath = "yolov3-tiny-obj_final.weights";

        public List<DetectedObject> GetDetectedObjectsFromVideo(string videoFileName = DefaultVideoFileName)
        {
            var arguments = $"detector demo {DataFilePath} {CfgFilePath} {WeightsFilePath} -dont_show -ext_output {videoFileName}";
            return RunDetectionAsync(arguments).GetAwaiter().GetResult();
        }

        public List<DetectedObject> GetDetectedObjectsFromImage(string imageFileName)
        {
            var arguments = $"detector test {DataFilePath} {CfgFilePath} {WeightsFilePath} -dont_show -ext_output {imageFileName}";
            return RunDetectionAsync(arguments).GetAwaiter().GetResult();
        }

        public List<DetectedObject> GetDetectedObjectsFromCamera()
        {
            var arguments = $"detector test {DataFilePath} {CfgFilePath} {WeightsFilePath} -dont_show -ext_output";
            return RunDetectionAsync(arguments).GetAwaiter().GetResult();
        }

        private async Task<List<DetectedObject>> RunDetectionAsync(string arguments)
        {
            var darknetYolo = Cli.Wrap("/app/darknet/darknet").WithArguments(arguments)
                .WithWorkingDirectory("/app/darknet");

            var detectedObjects = new List<DetectedObject>();

            await foreach (var cmdEvent in darknetYolo.ListenAsync())
            {
                if (!(cmdEvent is StandardOutputCommandEvent stdOut))
                {
                    continue;
                }

                if (stdOut.Text.Contains("trafficcone"))
                {
                    var numbers = Regex.Matches(stdOut.Text, "[0-9]{1,4}").OfType<Match>()
                        .Select(p => Convert.ToInt32(p.Value)).ToArray();

                    var confidence = (double) numbers[0] / 100;
                    var x = numbers[1];
                    var y = numbers[2];
                    var width = numbers[3];
                    var height = numbers[4];

                    detectedObjects.Add(new DetectedObject
                        {Name = "Traffic_Cone", Confidence = confidence, Location = new Rectangle(x, y, width, height)});

                }
                else if (detectedObjects.Any())
                {
                    detectedObjects.Clear();
                }
            }

            if (detectedObjects.Any())
            {
                detectedObjects.Clear();
            }

            return detectedObjects;
        }
    }
}

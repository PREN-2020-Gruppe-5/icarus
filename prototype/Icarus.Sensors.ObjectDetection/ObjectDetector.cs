using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using CliWrap;
using CliWrap.EventStream;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetector
    {
        private Action<List<DetectedObject>> _detectedObjectCallback;

        public void SetCallback(Action<List<DetectedObject>> callback)
        {
            _detectedObjectCallback = callback;
        }

        public async void StartDetection()
        {
            Console.WriteLine("Starting ObjectDetection with params 'detector demo data/obj.data cfg/yolov3-tiny-traffic_cone.cfg yolov3-tiny-obj_final.weights -dont_show -ext_output traffic_cones.mp4'");
            var darknetYolo = Cli.Wrap("/app/darknet/darknet")
            .WithArguments("detector demo data/obj.data cfg/yolov3-tiny-traffic_cone.cfg yolov3-tiny-obj_final.weights -dont_show -ext_output traffic_cones.mp4")
            .WithWorkingDirectory("/app/darknet");

            var detectedObjects = new List<DetectedObject>();

            await foreach (var cmdEvent in darknetYolo.ListenAsync())
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent started:
                        Console.WriteLine($"Process started; ID: {started.ProcessId}");
                        break;

                    case StandardOutputCommandEvent stdOut:
                        //Console.WriteLine($"Out> {stdOut.Text}");

                        if (stdOut.Text.Contains("trafficcone"))
                        {
                            var numbers = Regex.Matches(stdOut.Text, "[0-9]{1,4}")
                                .Select(p => Convert.ToInt32(p.Value))
                                .ToArray();

                            var confidence = (double)numbers[0] / 100;
                            var x = numbers[1];
                            var y = numbers[2];
                            var width = numbers[3];
                            var height = numbers[4];

                            detectedObjects.Add(new DetectedObject
                            {
                                Name = "trafficcone",
                                Confidence = confidence,
                                Location = new Rectangle(x, y, width, height)
                            });

                        }
                        else if (detectedObjects.Any())
                        {
                            _detectedObjectCallback?.Invoke(detectedObjects);

                            detectedObjects.Clear();
                        }

                        break;

                    case StandardErrorCommandEvent stdErr:
                        //Console.WriteLine($"Err> {stdErr.Text}");
                        break;

                    case ExitedCommandEvent exited:
                        //Console.WriteLine($"Process exited; Code: {exited.ExitCode}");
                        break;
                }
            }
        }

    }
}

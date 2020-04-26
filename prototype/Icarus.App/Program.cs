using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Icarus.Actuators.Motor;
using Icarus.Common;
using Icarus.Sensors.Motor;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.App
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            // check for running software on linux arm64 (nvidia jetson). if so, register real sensor drivers
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RuntimeInformation.OSArchitecture == Architecture.Arm64)
            {
                Console.WriteLine("Running on Nvidia Jetson. Initialize drivers...");

                MotorActor.Initialize(serviceCollection);
                DistanceSensor.Initialize(serviceCollection);
                TiltSensor.Initialize(serviceCollection);
                serviceCollection.AddSingleton(p =>
                {
                    var detector = new ObjectDetector();
                    detector.SetCallback(detectedObjects =>
                    {
                        foreach (var detectedObject in detectedObjects)
                        {
                            Console.WriteLine($"Traffic Cone detected: {detectedObject.Location.ToString()} ({detectedObject.Confidence})");
                        }
                    });

                    return detector;
                });
            }
            else
            {
                Console.WriteLine("Running on development machine. Registering fake services...");
                serviceCollection.AddSingleton<IMotorActor, SimulatedMotorActor>();
                serviceCollection.AddSingleton<IHallEffectController, SimulatedHallEffectController>();
                serviceCollection.AddTransient<ITiltSensor, ConfigurableTiltSensor>();
                serviceCollection.AddSingleton<ITiltConfiguration, TiltConfiguration>();
                serviceCollection.AddSingleton<IObjectDetector, RandomObjectDetector>();
            }

            var cancellationTokenSource = new CancellationTokenSource();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var objectDetector = serviceProvider.GetService<IObjectDetector>();
            var tiltSensor = serviceProvider.GetService<ITiltSensor>();
            var tiltConfiguration = serviceProvider.GetService<ITiltConfiguration>();
            var motorController = serviceProvider.GetService<IMotorActor>();
            var hallEffectSensorService = serviceProvider.GetService<IHallEffectController>();

            motorController.SetLeft(1);
            motorController.SetRight(1);

            // this task simulates driving over an obstacle every 11.5 sec.
            var tiltSimulationTask = Task.Run(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    tiltConfiguration.SetTilt(VehicleOrientation.Flat);
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);

                    await tiltConfiguration.SetTilt(VehicleOrientation.ClimbingUpObstacle, TimeSpan.FromSeconds(1));
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);

                    await tiltConfiguration.SetTilt(VehicleOrientation.Flat, TimeSpan.FromSeconds(1));
                    await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationTokenSource.Token);

                    await tiltConfiguration.SetTilt(VehicleOrientation.ClimbingDownObstacle, TimeSpan.FromSeconds(1));
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);

                    await tiltConfiguration.SetTilt(VehicleOrientation.Flat, TimeSpan.FromSeconds(1));
                }
            }, cancellationTokenSource.Token);

            objectDetector.SetCallback(detectedObjects =>
            {
                for (var i = 0; i < 5; i++)
                {
                    ConsoleHelper.WriteString($"Detected Object {i}", detectedObjects.Count <= i ? string.Empty : $"{detectedObjects[i].Location.ToString()} ({detectedObjects[i].Confidence:F})");
                }
            });

            // start object detection after 500ms
            var objectDetectionTask = Task.Delay(500, cancellationTokenSource.Token)
                .ContinueWith(a => objectDetector.RunDetectionAsync(string.Empty, CancellationToken.None), cancellationTokenSource.Token);

            var reportingTask = Task.Run(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    ConsoleHelper.WriteDouble("RPM Right", hallEffectSensorService.GetWheelRpm(WheelLocation.Right));
                    ConsoleHelper.WriteDouble("RPM Left", hallEffectSensorService.GetWheelRpm(WheelLocation.Left));

                    var tilt = tiltSensor.GetTilt();
                    ConsoleHelper.WriteDouble("Rotation X", tilt.RotationX);
                    ConsoleHelper.WriteDouble("Rotation Y", tilt.RotationY);

                    await Task.Delay(200, cancellationTokenSource.Token);
                }
            }, cancellationTokenSource.Token);

            await Task.WhenAll(tiltSimulationTask, objectDetectionTask, reportingTask);
        }
    }
}

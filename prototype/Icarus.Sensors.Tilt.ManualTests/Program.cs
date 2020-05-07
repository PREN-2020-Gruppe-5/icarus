using System;
using System.Runtime.InteropServices;
using Icarus.Common;
#if !DEBUG
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Icarus.Sensors.Tilt.ManualTests
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // sudo docker run -it --rm --name tilt --privileged derungsapp/icarus.sensors.tilt.manualtests

            // test if this test is run on ARM64 and Linux (Nvidia Jetson Nano)
            Console.WriteLine($"This test runs on {RuntimeInformation.OSDescription} {RuntimeInformation.ProcessArchitecture}");
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
                ConsoleHelper.WriteLine("This manual test is not supported on this machine. \n Please make sure to run the test on the actual device with the sensors wired. \n Press 'c' to continue in debug mode", ConsoleColor.Red);
                var key = Console.ReadKey();

                if (key.KeyChar != 'c')
                {
                    return;
                }
            }

#if !DEBUG
            var serviceCollection = new ServiceCollection();
            TiltModule.Initialize(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var tiltSensor = serviceProvider.GetService<ITiltSensor>();
#else
            ITiltSensor tiltSensor = null;
#endif


            TestTiltSensorFlat(tiltSensor, 5);
            TestTiltSensorFrontUp45Degrees(tiltSensor, 8);
            TestTiltSensorBackUp45Degrees(tiltSensor, 8);
            TestTiltSensorLeftSideUp45Degrees(tiltSensor, 8);
            TestTiltSensorRightSideUp45Degrees(tiltSensor, 8);
            TestTiltSensorRightSideUpWhileWheelie45Degrees(tiltSensor, 10);

        }

        private static int _testNumber = 1;

        private static void TestTiltSensorRightSideUpWhileWheelie45Degrees(ITiltSensor tiltSensor, int toleranceDegrees)
        {
            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine("Tilt sensor 45° to the left side like a stuntman with a car where the right wheels are no longer on the ground and do a wheelie 45° at the same time. This way only the wheel on the left back is on the ground. ");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var rotationResult = tiltSensor.GetRotationResult();
            TestTiltSensor(rotationResult, new RotationResult { XRotation = 45, YRotation = -45 }, toleranceDegrees);
        }

        private static void TestTiltSensorRightSideUp45Degrees(ITiltSensor tiltSensor, int toleranceDegrees)
        {
            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine("Tilt sensor 45° to the left side like a stuntman with a car where the right wheels are no longer on the ground");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var rotationResult = tiltSensor.GetRotationResult();
            TestTiltSensor(rotationResult, new RotationResult { XRotation = 0, YRotation = -45 }, toleranceDegrees);
        }

        private static void TestTiltSensorLeftSideUp45Degrees(ITiltSensor tiltSensor, int toleranceDegrees)
        {
            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine("Tilt sensor 45° to the right side like a stuntman with a car where the left wheels are no longer on the ground");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var rotationResult = tiltSensor.GetRotationResult();
            TestTiltSensor(rotationResult, new RotationResult { XRotation = 0, YRotation = 45 }, toleranceDegrees);
        }

        private static void TestTiltSensorFrontUp45Degrees(ITiltSensor tiltSensor, int toleranceDegrees)
        {
            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine("Do a wheelie with 45° while keeping the Y-axis at 0°");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var rotationResult = tiltSensor.GetRotationResult();
            TestTiltSensor(rotationResult, new RotationResult { XRotation = 45, YRotation = 0 }, toleranceDegrees);
        }

        private static void TestTiltSensorBackUp45Degrees(ITiltSensor tiltSensor, int toleranceDegrees)
        {
            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine("Do a stoppie with 45° while keeping the Y-axis at 0°");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var rotationResult = tiltSensor.GetRotationResult();
            TestTiltSensor(rotationResult, new RotationResult { XRotation = -45, YRotation = 0 }, toleranceDegrees);
        }


        private static void TestTiltSensorFlat(ITiltSensor tiltSensor, int toleranceDegrees)
        {
            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine("Place the TiltSensor on a flat surface");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var rotationResult = tiltSensor.GetRotationResult();
            TestTiltSensor(rotationResult, new RotationResult { XRotation = 0, YRotation = 0 }, toleranceDegrees);
        }

        private static void TestTiltSensor(RotationResult actualRotationResult, RotationResult expectedRotationResult, double toleranceDegrees)
        {
            // X axis
            Console.WriteLine($"Expected rotation X: {expectedRotationResult.XRotation}°. Actual rotation X: {actualRotationResult.XRotation}°. Tolerance: +{toleranceDegrees}/-{toleranceDegrees}");
            if (actualRotationResult.XRotation <= expectedRotationResult.XRotation + toleranceDegrees && actualRotationResult.XRotation >= expectedRotationResult.XRotation - toleranceDegrees)
            {
                ConsoleHelper.WriteLine($"Test {_testNumber} 'rotation X' passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine($"Test {_testNumber} 'rotation X' failed", ConsoleColor.Red);
            }

            // Y axis
            Console.WriteLine($"Expected rotation Y: {expectedRotationResult.YRotation}°. Actual rotation Y: {actualRotationResult.YRotation}°. Tolerance: +{toleranceDegrees}/-{toleranceDegrees}");
            if (actualRotationResult.YRotation <= expectedRotationResult.YRotation + toleranceDegrees && actualRotationResult.YRotation >= expectedRotationResult.YRotation - toleranceDegrees)
            {
                ConsoleHelper.WriteLine($"Test {_testNumber} 'rotation Y' passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine($"Test {_testNumber} 'rotation Y' failed", ConsoleColor.Red);
            }

            _testNumber++;
        }
    }
}

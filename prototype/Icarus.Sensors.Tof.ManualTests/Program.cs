using System;
using System.Runtime.InteropServices;
using Icarus.Common;

namespace Icarus.Sensors.Tof.ManualTests
{
    public class Program
    {
        private static void Main(string[] args)
        {

            // sudo docker run -it --rm --name tof --privileged derungsapp/icarus.sensors.tof.manualtests

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
            TofSensor.Initialize(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var tofSensor = serviceProvider.GetService<ITofSensor>();
#else
            ITofSensor tofSensor = null;
#endif

            TestDistanceSensor(tofSensor, 1000, 100);
            TestDistanceSensor(tofSensor, 500, 50);
            TestDistanceSensor(tofSensor, 300, 50);
            TestDistanceSensor(tofSensor, 0, 0);
        }

        private static int _testNumber = 1;

        private static void TestDistanceSensor(ITofSensor tofSensor, double expected, double tolerance)
        {
            // for debug purpose only
            var random = new Random();
            var max = expected + 2 * tolerance;
            var min = expected - 2 * tolerance;

            Console.WriteLine();
            ConsoleHelper.WriteLine($"Test {_testNumber}", ConsoleColor.Yellow);
            Console.WriteLine($"Place an obstacle, your hand or a wall about {expected}mm away from the TofSensor");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            var distance = tofSensor?.GetDistanceMillimeters() ?? random.Next((int) min, (int) max);
            Console.WriteLine($"Expected distance: {expected}mm. Actual distance: {distance}mm. Tolerance: +{tolerance}/-{tolerance}");

            if (distance <= expected + tolerance && distance >= expected - tolerance)
            {
                ConsoleHelper.WriteLine($"Test {_testNumber} passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine($"Test {_testNumber} failed", ConsoleColor.Red);
            }

            _testNumber++;
        }
    }
}

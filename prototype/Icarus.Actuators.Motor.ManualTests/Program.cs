using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Icarus.Common;
#if !DEBUG
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Icarus.Actuators.Motor.ManualTests
{

    class Program
    {
        static void Main(string[] args)
        {
            // sudo docker run -it --rm --name motor --privileged derungsapp/icarus.actuators.motor.manualtests

            // test if this test is run on ARM64 and Linux (Nvidia Jetson Nano)
            Console.WriteLine($"This test runs on {RuntimeInformation.OSDescription} {RuntimeInformation.ProcessArchitecture}");
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
                ConsoleHelper.WriteLine("This manual test is not supported on this machine. \nPlease make sure to run the test on the actual device with the sensors wired. \nPress 'c' to continue in debug mode", ConsoleColor.Red);
                var key = Console.ReadKey();

                if (key.Key != ConsoleKey.C)
                {
                    return;
                }
            }

#if !DEBUG
            var serviceCollection = new ServiceCollection();
            MotorModule.Initialize(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var motorActor = serviceProvider.GetService<IMotorActor>();
#else
            IMotorActor motorActor = null;
#endif
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            motorActor?.SetRight(0.5);
            Console.WriteLine("Right motor spinning forward? Press 'Enter' for yes");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                ConsoleHelper.WriteLine("Test right motor forward passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test right motor forward failed", ConsoleColor.Red);
            }

            motorActor?.SetRight(-0.5);
            Console.WriteLine("Right motor spinning backwards? Press 'Enter' for yes");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                ConsoleHelper.WriteLine("Test right motor backwards passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test right motor backwards failed", ConsoleColor.Red);
            }

            motorActor?.SetLeft(0.5);
            Console.WriteLine("Left motor spinning forward? Press 'Enter' for yes");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                ConsoleHelper.WriteLine("Test left motor forward passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test left motor forward failed", ConsoleColor.Red);
            }

            motorActor?.SetLeft(-0.5);
            Console.WriteLine("Left motor spinning backwards? Press 'Enter' for yes");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                ConsoleHelper.WriteLine("Test left motor backwards passed", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLine("Test left motor backwards failed", ConsoleColor.Red);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Icarus.Actuators.Motor;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            
            Console.WriteLine("starting");

            MotorController.Initialize(serviceCollection);
            DistanceSensor.Initialize(serviceCollection);
            TiltSensor.Initialize(serviceCollection);

            serviceCollection.AddSingleton(p =>
            {
                var objectDetector = new ObjectDetector();
                objectDetector.SetCallback(detectedObjects =>
                {
                    foreach (var detectedObject in detectedObjects)
                    {
                        Console.WriteLine($"Traffic Cone detected: {detectedObject.Location.ToString()} ({detectedObject.Confidence})");
                    }
                });

                return objectDetector;
            });
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var objectDetector = serviceProvider.GetService<ObjectDetector>();
            var motorController = serviceProvider.GetService<MotorController>();
            var distanceSensor = serviceProvider.GetService<DistanceSensor>();
            var tiltSensor = serviceProvider.GetService<TiltSensor>();
            var random = new Random();


            _ = Task.Run(() =>
              {
                  objectDetector.StartDetection();
              });

            while (true)
            {

                var distance = distanceSensor.GetDistance();
                Console.WriteLine($"Distance: {distance}mm");

                var tilt = tiltSensor.GetTilt();
                Console.WriteLine($"Acceleration X: {tilt.AccelerationX}");
                Console.WriteLine($"Acceleration Y: {tilt.AccelerationY}");
                Console.WriteLine($"Acceleration Z: {tilt.AccelerationZ}");
                Console.WriteLine($"Gyroscope X: {tilt.GyroscopeX}");
                Console.WriteLine($"Gyroscope Y: {tilt.GyroscopeY}");
                Console.WriteLine($"Gyroscope Z: {tilt.GyroscopeZ}");
                Console.WriteLine($"Rotation X: {tilt.RotationX}");
                Console.WriteLine($"Rotation Y: {tilt.RotationY}");

                motorController.SetLeft(random.NextDouble());
                motorController.SetRight(random.NextDouble());

                await Task.Delay(100);
            }

            //serviceCollection.AddMassTransit(x =>
            //{
            //    x.AddConsumer<TofConsumer>();
            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        cfg.Host("icarus.rabbitmq");
            //        cfg.ConfigureEndpoints(provider);
            //    }));
            //});

            //var serviceProvider = serviceCollection.BuildServiceProvider();

            //var busControl = serviceProvider.GetService<IBusControl>();

            //await busControl.StartAsync();

            //while (true)
            //{
            //    await Task.Delay(100);
            //}
        }
    }

    //public class TofConsumer : IConsumer<TofUpdatedNotification>
    //{
    //    private readonly IBusControl _busControl;

    //    public TofConsumer(IBusControl busControl)
    //    {
    //        _busControl = busControl;
    //    }

    //    public Task Consume(ConsumeContext<TofUpdatedNotification> context)
    //    {
    //        var distance = context.Message.DistanceMillimeters;

    //        _busControl.Publish(new SetMotorSpeedRequest { Left = distance <= 150 ? 0 : 0.5, Right = 0 });

    //        return TaskUtil.Completed;
    //    }
    //}
}

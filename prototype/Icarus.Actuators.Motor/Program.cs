//using System.Device.Gpio;
//using System.Device.Gpio.Drivers;
//using System.Device.Pwm;
//using System.Threading.Tasks;
//using MassTransit;
//using Microsoft.Extensions.DependencyInjection;

//namespace Icarus.Actuators.Motor
//{
//    class Program
//    {

//        static async Task Main(string[] args)
//        {
//            var serviceCollection = new ServiceCollection();

//            serviceCollection.AddMassTransit(x =>
//            {
//                x.AddConsumer<SetMotorSpeedRequestConsumer>();
//                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
//                {
//                    cfg.Host("icarus.rabbitmq");
//                    cfg.ConfigureEndpoints(provider);
//                }));
//            });
            


//            var serviceProvider = serviceCollection.BuildServiceProvider();
//            var busControl = serviceProvider.GetService<IBusControl>();

//            await busControl.StartAsync();

//            while (true)
//            {
//                await Task.Delay(100);
//            }
//        }
//    }
//}

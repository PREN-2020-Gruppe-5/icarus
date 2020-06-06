using System.Threading.Tasks;
using Icarus.Sensors.Button;
using Microsoft.Extensions.DependencyInjection;

namespace Icarus.App
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            AppModule.Initialize(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var deviceController = serviceProvider.GetService<DeviceController>();
            var buttonController = serviceProvider.GetService<ButtonController>();

            while (!buttonController.GetButtonPressed())
            {
                await Task.Delay(1);
            }

            await deviceController.Start();
        }
    }
}

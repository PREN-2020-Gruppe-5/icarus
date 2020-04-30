using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetectionModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IObjectDetectionSensor, ObjectDetectionSensor>();
            serviceCollection.AddSingleton<IObjectDetectionController, ObjectDetectionController>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Icarus.Sensors.ObjectDetection
{
    public class ObjectDetectionSimulationModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IObjectDetectionSensor, ObjectDetectionSensorSimulator>();
            serviceCollection.AddSingleton<IObjectDetectionController, ObjectDetectionController>();
        }
    }
}

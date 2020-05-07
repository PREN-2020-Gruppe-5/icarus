namespace Icarus.Sensors.ObjectDetection
{
    public interface IObjectDetectionController
    {
        DetectedObject GetNearestDetectedTrafficCone();
    }
}
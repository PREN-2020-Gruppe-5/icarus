using System;

namespace Icarus.Sensors.Tilt
{
    public class TiltController : ITiltController
    {
        private const double XAxisIncreasingLimit = 20;
        private const double XAxisDecreasingLimit = -20;
        private const double XAxisPositiveLimit = 180;
        private const double XAxisNegativeLimit = -180;

        private readonly ITiltSensor tiltSensor;

        public TiltController(ITiltSensor tiltSensor)
        {
            this.tiltSensor = tiltSensor;
        }

        public TiltResult GetTiltResult()
        {
            var rotationResult = this.tiltSensor.GetRotationResult();
            var orientationInformation = this.MapRotationResultToOrientationResult(rotationResult);

            return new TiltResult(orientationInformation);
        }

        private OrientationInformation MapRotationResultToOrientationResult(RotationResult rotationResult)
        {
            OrientationInformation orientationInformation;

            if (rotationResult.XRotation >= XAxisDecreasingLimit && rotationResult.XRotation <= XAxisIncreasingLimit)
            {
                orientationInformation = OrientationInformation.Horizontal;
            }
            else if (rotationResult.XRotation < XAxisDecreasingLimit && rotationResult.XRotation >= XAxisNegativeLimit)
            {
                orientationInformation = OrientationInformation.Decreasing;
            }
            else if (rotationResult.XRotation > XAxisIncreasingLimit && rotationResult.XRotation <= XAxisPositiveLimit)
            {
                orientationInformation = OrientationInformation.Increasing;
            }
            else
            {
                throw new InvalidOperationException($"Rotation X has a value ({rotationResult.XRotation}) which is out of the limits (-180° to 180°).");
            }

            return orientationInformation;
        }
    }
}

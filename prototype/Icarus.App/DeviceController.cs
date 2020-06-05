using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Icarus.Actuators.Motor;
using Icarus.Common;
using Icarus.Sensors.HallEffect;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;

namespace Icarus.App
{
    public class DeviceController
    {
        private readonly IMotorController motorController;
        private readonly IHallEffectController hallEffectController;
        private readonly IObjectDetectionController objectDetectionController;
        private readonly ITiltController tiltController;
        private readonly ITofController tofController;

        public DeviceController(IMotorController motorController, IHallEffectController hallEffectController, IObjectDetectionController objectDetectionController, ITiltController tiltController, ITofController tofController)
        {
            this.motorController = motorController;
            this.hallEffectController = hallEffectController;
            this.objectDetectionController = objectDetectionController;
            this.tiltController = tiltController;
            this.tofController = tofController;
        }

        public void Start()
        {

        }

        public async Task<DetectedObject> FaceNearestTrafficCone()
        {
            var detectedObjects = new Dictionary<int, DetectedObject>();

            // 360° turn in 30° steps --> 12*30
            for (var i = 0; i < (360 / 30); i++)
            {
                var nearestDetectedTrafficCone = this.objectDetectionController.GetNearestDetectedTrafficCone();
                detectedObjects.Add(i, nearestDetectedTrafficCone);
                await this.motorController.TurnLeftAsync();
            }

            var (angleIndex, nearestObject) = detectedObjects.OrderByDescending(_ => _.Value.Location.Width * _.Value.Location.Height).FirstOrDefault();

            // turn until we face the nearestObject
            for (var i = 0; i < angleIndex; i++)
            {
                await this.motorController.TurnLeftAsync();
            }

            return nearestObject;
        }

        public async Task ApproachNearestVisibleTrafficCone(DetectedObject nearestDetectedObject)
        {
            // https://imgur.com/a/KwNsgFa
            // https://imgur.com/a/ugY9CRE  6m distance & bbox >= 33%
            /*
            [17:51, 5/8/2020] Robin Derungs: 1. Auf 42% korrigieren
                                                Fahren bis bbox >=33% über 5-6 samples
                                                Dann erneut ausrichten auf 33% und solange fahren bis links pylon durch tof
            [17:51, 5/8/2020] Robin Derungs: 2. Sobald links erkannt, fahre noch ca. 50cm weiter.
                                                Falls tilt not horizontal, fahre weiter bis horizontal
            [17:53, 5/8/2020] Robin Derungs: 3. Drehe bis ein neuer pylon >= 50% pic height gefunden.
            [17:55, 5/8/2020] Robin Derungs: 4. Keiner gefunden? Dh wird durch den aktuellen verdeckt. Drehe 45 grad links zu. Fahre fort mit 5.
                                                Gefunden? Fahre fort mit 1.
            [17:56, 5/8/2020] Robin Derungs: 5. Fahre 1 m
            [17:56, 5/8/2020] Robin Derungs: 6. Fahre fort mit 3.
             */

            // Ausrichten 42% +/- 2%
            while (!this.GetAnglePercentOfTrafficConeCenter(nearestDetectedObject).IsWithin(0.40, 0.44))
            {
                await this.motorController.TurnLeftAsync();
                nearestDetectedObject = this.objectDetectionController.GetNearestDetectedTrafficCone();
            }

            // bbox size can vary quite much frame by frame. so check the average size over 6 measurements
            // use a circular buffer initialized with zeros so it requires all 6 measurements to be done
            var circularBuffer = new CircularBuffer<double>(6, Enumerable.Repeat(0d, 6).ToArray());
            while (circularBuffer.Average() < 0.33)
            {
                // TODO: maybe add delay so that we don't flood motors with signals
                this.motorController.SetForward(MotorSpeed.Medium);
                nearestDetectedObject = this.objectDetectionController.GetNearestDetectedTrafficCone();
                circularBuffer.PushFront(this.GetBboxHeightPercentage(nearestDetectedObject));
            }

            // Ausrichten 33% +/- 2%
            while (!this.GetAnglePercentOfTrafficConeCenter(nearestDetectedObject).IsWithin(0.31, 0.35))
            {
                await this.motorController.TurnLeftAsync();
                nearestDetectedObject = this.objectDetectionController.GetNearestDetectedTrafficCone();
            }

            while (this.tofController.GetTofResult().DistanceInformation != DistanceInformation.TrafficConeDetected)
            {
                // TODO: maybe add delay so that we don't flood motors with signals
                this.motorController.SetForward(MotorSpeed.Medium);
            }

            // drive approx 50cm. can be 40 or 60, doesnt really matter that much
            this.motorController.SetForward(MotorSpeed.Slow);
            await Task.Delay(TimeSpan.FromSeconds(2));

            while (this.tiltController.GetTiltResult().OrientationInformation != OrientationInformation.Horizontal)
            {
                // TODO: maybe add delay
                this.motorController.SetForward(MotorSpeed.Slow);
            }

            this.motorController.Stop();
        }

        private double GetAnglePercentOfTrafficConeCenter(DetectedObject detectedObject)
        {
            var centerX = (detectedObject.Location.X + detectedObject.Location.Width) / 2d;
            return centerX * (1d / 1920d);
        }

        private double GetBboxHeightPercentage(DetectedObject detectedObject)
        {
            return detectedObject.Location.Height * (1d / 1080d);
        }
    }
}

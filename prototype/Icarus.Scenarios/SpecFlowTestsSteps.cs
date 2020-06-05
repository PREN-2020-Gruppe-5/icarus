using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using FluentAssertions;
using Icarus.Actuators.Motor;
using Icarus.App;
using Icarus.Sensors.ObjectDetection;
using Icarus.Sensors.Tilt;
using Icarus.Sensors.Tof;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Icarus.Scenarios
{
    [Binding]
    public class SpecFlowTestsSteps
    {
        private readonly ServiceProvider serviceProvider;


        public SpecFlowTestsSteps()
        {
            var serviceCollection = new ServiceCollection();
            SimulationModule.Initialize(serviceCollection);
            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Given(@"the vehicle is driving on the parcours")]
        public void GivenTheVehicleIsDrivingOnTheParcours()
        {
            var deviceController = this.serviceProvider.GetService<DeviceController>();
            deviceController.Start();

            var motorController = (MotorControllerSimulator)this.serviceProvider.GetService<IMotorController>();
            motorController.SetForward(0.3, 0.3);
        }

        [When(@"the tilt sensor measures an incline")]
        public void WhenTheTiltSensorMeasuresAnIncline()
        {
            var tiltSensor = (TiltSensorSimulator)this.serviceProvider.GetService<ITiltSensor>();
            tiltSensor.SetRotationResult(35, 0);
        }

        [When(@"the tilt sensor measures a decline")]
        public void WhenTheTiltSensorMeasuresADecline()
        {
            var tiltSensor = (TiltSensorSimulator)this.serviceProvider.GetService<ITiltSensor>();
            tiltSensor.SetRotationResult(-35, 0);
        }

        [When(@"the tilt sensor measures horizonally")]
        public void WhenTheTiltSensorMeasuresHorizonally()
        {
            var tiltSensor = (TiltSensorSimulator)this.serviceProvider.GetService<ITiltSensor>();
            tiltSensor.SetRotationResult(0, 0);
        }
        
        [When(@"the tof sensor returns values which triggers reaction")]
        public void WhenTheTofSensorReturnsValuesWhichTriggersReaction()
        {
            var tofSensor = (TofSensorSimulator)this.serviceProvider.GetService<ITofSensor>();
            tofSensor.SetDistanceInMillimeters(200);
        }

        [When(@"the tof sensor returns values which does not trigger reaction")]
        public void WhenTheTofSensorReturnsValuesWhichDoesNotTriggerReaction()
        {
            var tofSensor = (TofSensorSimulator)this.serviceProvider.GetService<ITofSensor>();
            tofSensor.SetDistanceInMillimeters(600);
        }

        [When(@"the tof sensor returns values which triggers a stop")]
        public void WhenTheTofSensorReturnsValuesWhichTriggersAStop()
        {
            var tofSensor = (TofSensorSimulator)this.serviceProvider.GetService<ITofSensor>();
            tofSensor.SetDistanceInMillimeters(50);
        }
        
        [When(@"the object detection returns values which triggers reaction")]
        public void WhenTheObjectDetectionReturnsValuesWhichTriggersReaction()
        {
            var objectDetectionSensor = (ObjectDetectionSensorSimulator)this.serviceProvider.GetService<IObjectDetectionSensor>();
            var detectedObjectOutOfLeftRange = new DetectedObject{Name = "Traffic_Cone_Vertical", Confidence = 0.8, Location = new Rectangle(250, 50, 50, 70)};
            objectDetectionSensor.SetDetectedObjects(new List<DetectedObject>{ detectedObjectOutOfLeftRange });
        }

        [When(@"the object detection returns values which does not trigger reaction")]
        public void WhenTheObjectDetectionReturnsValuesWhichDoesNotTriggerReaction()
        {
            var objectDetectionSensor = (ObjectDetectionSensorSimulator)this.serviceProvider.GetService<IObjectDetectionSensor>();
            var detectedObjectInLeftRange = new DetectedObject { Name = "Traffic_Cone_Vertical", Confidence = 0.8, Location = new Rectangle(50, 50, 50, 70) };
            objectDetectionSensor.SetDetectedObjects(new List<DetectedObject> { detectedObjectInLeftRange });
        }

        [When(@"the object detection returns values which triggers a stop")]
        public void WhenTheObjectDetectionReturnsValuesWhichTriggersAStop()
        {
            var objectDetectionSensor = (ObjectDetectionSensorSimulator)this.serviceProvider.GetService<IObjectDetectionSensor>();
            var detectedObjectInLeftRange = new DetectedObject { Name = "Traffic_Cone_Horizontal", Confidence = 0.8, Location = new Rectangle(50, 50, 50, 70) };
            objectDetectionSensor.SetDetectedObjects(new List<DetectedObject> { detectedObjectInLeftRange });
        }

        [Then(@"reaction is triggered")]
        public void ThenReactionIsTriggered()
        {
            Thread.Sleep(200);
            var motorController = (MotorControllerSimulator)this.serviceProvider.GetService<IMotorController>();
            motorController.GetSpeedOfRightMotorActor().Should().Be(0.3);
            motorController.GetSpeedOfLeftMotorActor().Should().Be(0.2);
        }

        [Then(@"no reaction is triggered")]
        public void ThenNoReactionIsTriggered()
        {
            Thread.Sleep(200);
            var motorController = (MotorControllerSimulator)this.serviceProvider.GetService<IMotorController>();
            motorController.GetSpeedOfRightMotorActor().Should().Be(0.3);
            motorController.GetSpeedOfLeftMotorActor().Should().Be(0.3);
        }

        [Then(@"the vehicle stops")]
        public void ThenTheVehicleStops()
        {
            Thread.Sleep(200);
            var motorController = (MotorControllerSimulator)this.serviceProvider.GetService<IMotorController>();
            motorController.GetSpeedOfRightMotorActor().Should().Be(0);
            motorController.GetSpeedOfLeftMotorActor().Should().Be(0);
        }

    }
}

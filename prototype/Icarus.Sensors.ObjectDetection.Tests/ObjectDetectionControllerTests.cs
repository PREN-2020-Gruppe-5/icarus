using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using Moq;
using Xunit;

namespace Icarus.Sensors.ObjectDetection.Tests
{
    public class ObjectDetectionControllerTests
    {
        private const string TrafficConeName = "Traffic_Cone";

        [Fact]
        public void ObjectDetectionController_GetNearestDetectedTrafficCone_WithoutDetectedObjects_ReturnsNull()
        {
            // Arrange
            var objectDetectionSensor = new Mock<IObjectDetectionSensor>();
            var detectedObjects = new List<DetectedObject>();
            objectDetectionSensor.Setup(_ => _.GetDetectedObjectsFromCamera()).Returns(detectedObjects);

            var testee = new ObjectDetectionController(objectDetectionSensor.Object);

            // Act
            var result = testee.GetNearestDetectedTrafficCone();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ObjectDetectionController_GetNearestDetectedTrafficCone_WithOneDetectedObjectNotTrafficCone_ReturnsNull()
        {
            // Arrange
            var objectDetectionSensor = new Mock<IObjectDetectionSensor>();
            var detectedObjects = new List<DetectedObject>
            {
                new DetectedObject{Name = "Banana", Confidence = 1, Location = new Rectangle()}
            };
            objectDetectionSensor.Setup(_ => _.GetDetectedObjectsFromCamera()).Returns(detectedObjects);

            var testee = new ObjectDetectionController(objectDetectionSensor.Object);

            // Act
            var result = testee.GetNearestDetectedTrafficCone();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ObjectDetectionController_GetNearestDetectedTrafficCone_WithOneDetectedObjectTrafficConeConfidenceToSmall_ReturnsNull()
        {
            // Arrange
            var objectDetectionSensor = new Mock<IObjectDetectionSensor>();
            var detectedObjects = new List<DetectedObject>
            {
                new DetectedObject{Name = TrafficConeName, Confidence = 0.59, Location = new Rectangle()}
            };
            objectDetectionSensor.Setup(_ => _.GetDetectedObjectsFromCamera()).Returns(detectedObjects);

            var testee = new ObjectDetectionController(objectDetectionSensor.Object);

            // Act
            var result = testee.GetNearestDetectedTrafficCone();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ObjectDetectionController_GetNearestDetectedTrafficCone_WithOneValidDetectedObjectTrafficCone_ReturnsExpectedTrafficCone()
        {
            // Arrange
            var objectDetectionSensor = new Mock<IObjectDetectionSensor>();
            var validTrafficCone = new DetectedObject { Name = TrafficConeName, Confidence = 0.6, Location = new Rectangle() };
            var detectedObjects = new List<DetectedObject> { validTrafficCone };
            objectDetectionSensor.Setup(_ => _.GetDetectedObjectsFromCamera()).Returns(detectedObjects);

            var testee = new ObjectDetectionController(objectDetectionSensor.Object);

            // Act
            var result = testee.GetNearestDetectedTrafficCone();

            // Assert
            result.Should().Be(validTrafficCone);
        }

        [Fact]
        public void ObjectDetectionController_GetNearestDetectedTrafficCone_WithMultipleValidDetectedObjectTrafficCones_ReturnsLargestTrafficCone()
        {
            // Arrange
            var objectDetectionSensor = new Mock<IObjectDetectionSensor>();
            var smallTrafficCone = new DetectedObject { Name = TrafficConeName, Confidence = 0.8, Location = new Rectangle { Height = 1, Width = 5 } };
            var mediumTrafficCone = new DetectedObject { Name = TrafficConeName, Confidence = 0.7, Location = new Rectangle { Height = 5, Width = 10 } };
            var largeTrafficCone = new DetectedObject { Name = TrafficConeName, Confidence = 0.6, Location = new Rectangle { Height = 10, Width = 15 } };
            var detectedObjects = new List<DetectedObject> { smallTrafficCone, largeTrafficCone, mediumTrafficCone };
            objectDetectionSensor.Setup(_ => _.GetDetectedObjectsFromCamera()).Returns(detectedObjects);

            var testee = new ObjectDetectionController(objectDetectionSensor.Object);

            // Act
            var result = testee.GetNearestDetectedTrafficCone();

            // Assert
            result.Should().Be(largeTrafficCone);
        }
    }
}

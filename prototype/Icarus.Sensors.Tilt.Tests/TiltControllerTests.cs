using System;
using FluentAssertions;
using Moq;
using Xunit;

namespace Icarus.Sensors.Tilt.Tests
{
    public class TiltControllerTests
    {
        [Theory]
        [InlineData(-180, OrientationInformation.Decreasing)]
        [InlineData(-35, OrientationInformation.Decreasing)]
        [InlineData(-20.000001, OrientationInformation.Decreasing)]
        [InlineData(-20, OrientationInformation.Horizontal)]
        [InlineData(0, OrientationInformation.Horizontal)]
        [InlineData(20, OrientationInformation.Horizontal)]
        [InlineData(20.000001, OrientationInformation.Increasing)]
        [InlineData(35, OrientationInformation.Increasing)]
        [InlineData(180, OrientationInformation.Increasing)]
        public void TiltController_GetTiltResults_WhenXRotationValid_ReturnsExpectedTiltResult(double xRotation, OrientationInformation expectedOrientationInformation)
        {
            // Arrange
            var tiltSensor = new Mock<ITiltSensor>();
            tiltSensor.Setup(_ => _.GetRotationResult()).Returns(new RotationResult {XRotation = xRotation});

            var testee = new TiltController(tiltSensor.Object);

            // Act
            var result = testee.GetTiltResult();

            // Assert
            result.OrientationInformation.Should().Be(expectedOrientationInformation);
        }

        [Theory]
        [InlineData(-180.000001)]
        [InlineData(180.000001)]
        public void TiltController_GetTiltResults_WhenXRotationInvalid_ThrowsInvalidOperationException(double xRotation)
        {
            // Arrange
            var tiltSensor = new Mock<ITiltSensor>();
            tiltSensor.Setup(_ => _.GetRotationResult()).Returns(new RotationResult { XRotation = xRotation });

            var testee = new TiltController(tiltSensor.Object);

            // Act
            Action a = () => testee.GetTiltResult();

            // Assert
            a.Should().Throw<InvalidOperationException>();
        }
    }
}

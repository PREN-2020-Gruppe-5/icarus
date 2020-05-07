using System;
using FluentAssertions;
using Icarus.Common;
using Moq;
using Xunit;

namespace Icarus.Sensors.HallEffect.Tests
{
    public class HallEffectControllerTests
    {
        [Theory]
        [InlineData(0, true, 0)]
        [InlineData(0, false, 0)]
        [InlineData(0.5, true, 500)]
        [InlineData(0.5, false, -500)]
        [InlineData(1, true, 1000)]
        [InlineData(1, false, -1000)]
        public void HallEffectController_GetTiltResult_ReturnsExpectedTiltResult(double hallEffectSensorDutyCycleA, bool forward, int expectedRpm)
        {
            // Arrange
            var hallEffectSensor = new Mock<IHallEffectSensor>();
            hallEffectSensor.Setup(_ => _.GetHallEffectSensorResult()).Returns(new HallEffectSensorResult { DutyCycleA = hallEffectSensorDutyCycleA, Forward = forward});

            var testee = new HallEffectController(new Directional<IHallEffectSensor>(hallEffectSensor.Object, hallEffectSensor.Object));

            // Act
            var result = testee.GetWheelRpm(WheelLocation.Left);

            // Assert
            result.Should().Be(expectedRpm);
        }
    }
}

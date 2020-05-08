using FluentAssertions;
using Xunit;

namespace Icarus.Actuators.Motor.Tests
{
    public class MotorSpeedConverterTests
    {
        [Theory]
        [InlineData(MotorSpeed.Slow, 0.2)]
        [InlineData(MotorSpeed.Medium, 0.5)]
        [InlineData(MotorSpeed.Fast, 0.8)]
        [InlineData(MotorSpeed.Maximum, 1)]
        public void MotorSpeedConverter_GetDutyCycleFromSpeed_ReturnsExpectedDutyCycle(MotorSpeed motorSpeed, double expectedDutyCycle)
        {
            // Arrange
            var testee = new MotorSpeedConverter();

            // Act
            var result = testee.GetDutyCycleFromSpeed(motorSpeed);

            // Assert
            result.Should().Be(expectedDutyCycle);
        }
    }
}

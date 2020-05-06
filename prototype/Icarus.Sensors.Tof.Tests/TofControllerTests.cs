using FluentAssertions;
using Moq;
using Xunit;

namespace Icarus.Sensors.Tof.Tests
{
    public class TofControllerTests
    {
        [Theory]
        [InlineData(-10, -10, DistanceInformation.Negative)]
        [InlineData(-0.000001, -0.000001, DistanceInformation.Negative)]
        [InlineData(0, 0, DistanceInformation.NearMiss)]
        [InlineData(10, 10, DistanceInformation.NearMiss)]
        [InlineData(99.999999, 99.999999, DistanceInformation.NearMiss)]
        [InlineData(100, 100, DistanceInformation.TrafficConeDetected)]
        [InlineData(300, 300, DistanceInformation.TrafficConeDetected)]
        [InlineData(499.999999, 499.999999, DistanceInformation.TrafficConeDetected)]
        [InlineData(500, 500, DistanceInformation.Void)]
        [InlineData(1500, 1500, DistanceInformation.Void)]
        public void TofController_GetTofResult_ReturnsExpectedTofResult(double tofSensorDistanceInMillimeters, double expectedDistanceInMillimeters, DistanceInformation expecteDistanceInformation)
        {
            // Arrange
            var tofSensor = new Mock<ITofSensor>();
            tofSensor.Setup(_ => _.GetDistanceInMillimeters()).Returns(tofSensorDistanceInMillimeters);

            var testee = new TofController(tofSensor.Object);

            // Act
            var result = testee.GetTofResult();

            // Assert
            result.DistanceInMillimeters.Should().Be(expectedDistanceInMillimeters);
            result.DistanceInformation.Should().Be(expecteDistanceInformation);
        }
    }
}

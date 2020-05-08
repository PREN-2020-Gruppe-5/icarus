using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Icarus.Common;
using Moq;
using Xunit;

namespace Icarus.Actuators.Motor.Tests
{
    public class MotorControllerTests
    {
        [Theory]
        [InlineData(MotorSpeed.Maximum)]
        [InlineData(MotorSpeed.Fast)]
        [InlineData(MotorSpeed.Medium)]
        [InlineData(MotorSpeed.Slow)]
        public void MotorController_SetForward_CallsMotorActorSetSpeedOnceOnEachSide(MotorSpeed motorSpeed)
        {
            // Arrange
            var motorActors = new Mock<IDirectional<IMotorActor>>();
            var motorActorLeft = new Mock<IMotorActor>();
            var motorActorRight = new Mock<IMotorActor>();
            motorActors.SetupGet(_ => _.Right).Returns(motorActorRight.Object);
            motorActors.SetupGet(_ => _.Left).Returns(motorActorLeft.Object);

            var motorSpeedConverter = new MotorSpeedConverter();
            var testee = new MotorController(motorActors.Object, motorSpeedConverter);

            // Act
            testee.SetForward(motorSpeed);

            // Assert
            motorActors.Verify(p => p.Right.SetSpeed(It.IsAny<double>()), Times.Once());
            motorActors.Verify(p => p.Left.SetSpeed(It.IsAny<double>()), Times.Once());
        }

        [Fact]
        public void MotorController_Stop_CallsMotorActorSetSpeedOnceOnEachSideWithZero()
        {
            // Arrange
            var motorActors = new Mock<IDirectional<IMotorActor>>();
            var motorActorLeft = new Mock<IMotorActor>();
            var motorActorRight = new Mock<IMotorActor>();
            motorActors.SetupGet(_ => _.Right).Returns(motorActorRight.Object);
            motorActors.SetupGet(_ => _.Left).Returns(motorActorLeft.Object);

            var motorSpeedConverter = new MotorSpeedConverter();
            var testee = new MotorController(motorActors.Object, motorSpeedConverter);

            // Act
            testee.Stop();

            // Assert
            motorActors.Verify(p => p.Right.SetSpeed(0), Times.Once());
            motorActors.Verify(p => p.Left.SetSpeed(0), Times.Once());
        }
    }
}

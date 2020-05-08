using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icarus.Common;

namespace Icarus.Actuators.Motor
{
    public class MotorController : IMotorController
    {
        private readonly IDirectional<IMotorActor> motorActors;
        private readonly IMotorSpeedConverter motorSpeedConverter;
        private MotorSpeed requestedSpeed = 0;

        public MotorController(IDirectional<IMotorActor> motorActors, IMotorSpeedConverter motorSpeedConverter)
        {
            this.motorActors = motorActors;
            this.motorSpeedConverter = motorSpeedConverter;
        }

        public double GetRequestedSpeedDutyCycle()
        {
            return motorSpeedConverter.GetDutyCycleFromSpeed(requestedSpeed);
        }

        public void SetForward(MotorSpeed motorSpeed)
        {
            requestedSpeed = motorSpeed;
            var requestedSpeedDutyCycle = motorSpeedConverter.GetDutyCycleFromSpeed(motorSpeed);
            motorActors.Right.SetSpeed(requestedSpeedDutyCycle);
            motorActors.Left.SetSpeed(requestedSpeedDutyCycle);
        }

        public void SetForward(double speedRight, double speedLeft)
        {
            motorActors.Right.SetSpeed(speedRight);
            motorActors.Left.SetSpeed(speedLeft);
        }

        public void Stop()
        {
            motorActors.Right.SetSpeed(0);
            motorActors.Left.SetSpeed(0);
        }

        public async Task TurnLeftAsync()
        {
            // not fully implemented. needs some math first. idea: both in different directions with low speed for 500ms
            motorActors.Right.SetSpeed(0.2);
            motorActors.Right.SetSpeed(-0.2);

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Stop();
        }
    }
}
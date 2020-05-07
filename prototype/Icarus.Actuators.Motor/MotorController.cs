using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Icarus.Common;

namespace Icarus.Actuators.Motor
{
    public class MotorController : IMotorController
    {
        private readonly IDirectional<IMotorActor> motorActors;
        private MotorSpeed requestedSpeed = 0;

        public MotorController(IDirectional<IMotorActor> motorActors)
        {
            this.motorActors = motorActors;
        }

        public double GetRequestedSpeedDutyCycle()
        {
            return GetDutyCycleFromSpeed(requestedSpeed);
        }

        public void SetForward(MotorSpeed motorSpeed)
        {
            requestedSpeed = motorSpeed;
            var requestedSpeedDutyCycle = GetDutyCycleFromSpeed(motorSpeed);
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

        private double GetDutyCycleFromSpeed(MotorSpeed motorSpeed)
        {
            switch (motorSpeed)
            {
                case MotorSpeed.Slow:
                    return 0.2;
                case MotorSpeed.Medium:
                    return 0.5;
                case MotorSpeed.Fast:
                    return 0.8;
                case MotorSpeed.Maximum:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(motorSpeed), motorSpeed, null);
            }
        }

    }
}
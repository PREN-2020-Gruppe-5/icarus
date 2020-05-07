using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Icarus.Actuators.Motor;
using Icarus.Sensors.HallEffect;

namespace Icarus.App
{
    public class FeedbackControlSystem
    {
        private readonly IMotorController motorController;
        private readonly IHallEffectController hallEffectController;

        public FeedbackControlSystem(IMotorController motorController, IHallEffectController hallEffectController)
        {
            this.motorController = motorController;
            this.hallEffectController = hallEffectController;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // note for myself: 0.5 == 400 rpm. 1.0 = 800 rpm
                    var requestedRpm = motorController.GetRequestedSpeedDutyCycle() * 800;
                    var leftHallEffectSensorResult = hallEffectController.GetWheelRpm(WheelLocation.Left);
                    var rightHallEffectSensorResult = hallEffectController.GetWheelRpm(WheelLocation.Right);

                    var differenceLeft = requestedRpm - leftHallEffectSensorResult;
                    var differenceRight = requestedRpm - rightHallEffectSensorResult;

                    motorController.SetForward(requestedRpm + differenceRight, requestedRpm + differenceLeft);

                    // update speed every 50ms, uses delta to set value
                    await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
                }

            }, cancellationToken);
        }
    }
}

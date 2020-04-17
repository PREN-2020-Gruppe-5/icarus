//using System;
//using System.Device.Pwm;
//using System.Threading.Tasks;
//using Icarus.Common.Contracts.Requests;
//using MassTransit;
//using MassTransit.Util;

//namespace Icarus.Actuators.Motor
//{
//    public class SetMotorSpeedRequestConsumer : IConsumer<SetMotorSpeedRequest>
//    {
//        private readonly MotorController _motorController;

//        public SetMotorSpeedRequestConsumer(MotorController motorController)
//        {
//            _motorController = motorController;
//        }

//        public Task Consume(ConsumeContext<SetMotorSpeedRequest> context)
//        {
//            Console.WriteLine($"Message Received: Left: {context.Message.Left} Right: {context.Message.Right}");
//            _motorController.SetLeft(Math.Round(context.Message.Left ?? 0, 2));
//            return TaskUtil.Completed;
//        }
//    }
//}
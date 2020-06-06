using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Sensors.Button
{
    public class ButtonModule
    {
        public static void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IButtonSensor, ButtonSensor>();
            serviceCollection.AddSingleton<IButtonController, ButtonController>();
        }
    }
}

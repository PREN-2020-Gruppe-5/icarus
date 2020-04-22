using System;
using System.Collections.Generic;
using System.Text;

namespace Icarus.Common
{
    public class TransformationEfficiencyProvider : ITransformationEfficiencyProvider
    {


        private double _currentEfficiency;

        public double GetEfficiency()
        {
            return _currentEfficiency;
        }

        public void SetEfficiency(double efficiency)
        {
            _currentEfficiency = efficiency;
        }
    }
}

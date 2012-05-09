using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Model;

namespace Restrictor
{
    class BasicRestrictor : IRestrictor
    {
        public AnglePositions MakeSafe(AnglePositions angles)
        {
            return angles;
        }
    }
}

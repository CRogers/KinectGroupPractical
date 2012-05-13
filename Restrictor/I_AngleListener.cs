using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restrictor;

namespace RobotSimulator
{
    public interface I_AngleListener
    {
        void kinectAngles(AnglePositions angles);

        void robotReady();

        void robotStopped();
    }
}

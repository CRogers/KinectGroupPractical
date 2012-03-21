using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotSimulator.Utility
{
    class MoreMaths
    {
        //For converting angles:
        public static double DegToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadToDeg(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}

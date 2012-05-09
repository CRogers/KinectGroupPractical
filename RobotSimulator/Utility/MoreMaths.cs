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
        //Returns the angle to set a motor to - 0 to 255 -, to replicate the given degrees - 0 to 360 degrees -
        public static double toMotorAngle(double angle)
        {
            return ((angle * 255) / 360);
        }

        //Returns the original angle - 0 to 360 degrees - represented by the motor angle - 0 to 255 -
        public static double fromMotorAngle(double angle)
        {
            return ((angle * 360) / 255);
        }
    }
}

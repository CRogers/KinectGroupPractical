using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NKH.MindSqualls;

namespace RobotControllerLib
{
    public struct MotorPort
    {
        public NxtMotorPort Motor { get; private set; }
        public int Brick { get; private set; }

        public MotorPort(NxtMotorPort motor, int brick) : this()
        {
            Motor = motor;
            Brick = brick;
        }
    }
}

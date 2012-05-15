using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RobotControllerLib;
using NKH.MindSqualls;

namespace RobotControllerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.ToStdout = true;
            
            var r = new Robot();
            Thread.Sleep(1000);
            r.LeftArm.ShoulderOut.TargetAngle = 90;
            //r.RightArm.ElbowAlong.TargetAngle = 90;
            //r.RightArm.ShoulderAlong.TargetAngle = 90;
            Console.ReadLine();

            r.Disconnect();
        }
    }
}

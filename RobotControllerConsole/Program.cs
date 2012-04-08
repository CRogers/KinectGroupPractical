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
            var brick = new NxtBrick(NxtCommLinkType.Bluetooth, 3);

            var motorA = new NxtMotor();
            brick.MotorA = motorA;

            brick.Connect();
            Console.WriteLine("Connected to NXTBrick");

            motorA.ResetMotorPosition(false);
            motorA.PollInterval = 5;
            motorA.Poll();
            Console.WriteLine(motorA.PollInterval);

            for (int i = 0; i < 5; i++) {
                Console.WriteLine("start");
                motorA.RunUntil(100, 800);
                Console.WriteLine("end");
            }
            /*
            int rot = 0;
            byte rotStep = 100;
            while(true) {
                motorA.ResetMotorPosition(false);
                Console.Write("Motor run @ {0} degrees... ", rot += rotStep);
                motorA.Run(75, rotStep);
                //motorA.Brake();
                Console.WriteLine("done (Tacho: {0}, mod: {1})", motorA.TachoCount, motorA.TachoCount%360);
                Thread.Sleep(2000);
                motorA.Brake();
            }*/
        }
    }

    public static class NxtMotorExtensions
    {
        public static void RunUntil(this NxtMotor motor, sbyte power, uint degrees)
        {
            motor.Poll();
            var initTacho = motor.TachoCount.Value;
            var endTacho = (uint)(initTacho + degrees);

            Console.WriteLine("tac: start: {0}, end: {1}", initTacho, endTacho);

            Console.WriteLine("running");
            motor.Run(power, 0);

            Console.WriteLine("waiting");
            while (true) {
                Console.WriteLine(motor.TachoCount - initTacho);
                if (motor.TachoCount + 90 >= endTacho)
                    break;
                Thread.Sleep(motor.PollInterval);
            }
            motor.Brake();
            Thread.Sleep(1000);
            Console.WriteLine("fin wait {0}", motor.TachoCount - initTacho);
            Thread.Sleep(1000);
        }
    }
}

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
            
            var r = new Robot();
            Thread.Sleep(1000);
            r.LeftArm.ShoulderOut.TargetAngle = 90;
            Console.ReadLine();

            r.Disconnect();

            /*var brick = new NxtBrick(NxtCommLinkType.Bluetooth, 3);

            var motorA = new NxtMotor();
            brick.MotorA = motorA;

            var motorB = new NxtMotor();
            brick.MotorB = motorB;

            var motorC = new NxtMotor();
            brick.MotorC = motorC;

            brick.Connect();
            Console.WriteLine("Connected to NXTBrick");

            motorA.ResetMotorPosition(false);
            motorA.PollInterval = 5;
            motorA.Poll();

            motorB.ResetMotorPosition(false);
            motorB.PollInterval = 5;
            motorB.Poll();

            motorC.ResetMotorPosition(false);
            motorC.PollInterval = 5;
            motorC.Poll();

            Joint b = new Joint(motorB);
            b.TargetAngle = 30;
            b.DegreeScaleFactor = 24;

            Joint c = new Joint(motorC);
            c.TargetAngle = 40;
            c.DegreeScaleFactor = 24;
            
            Console.ReadLine();
            motorA.Brake();
            motorB.Brake();
            motorC.Brake();

            brick.Disconnect();

            /*
            // Get angle
            Console.WriteLine("{0}, {1}", motorB.TachoCount, motorB.TachoCount % 360);

            for (int i = 0; i < 5; i++) {
                Console.WriteLine("start");
                motorB.RunUntil(-100, 720);
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
            while (motor.TachoCount + 30 < endTacho) {
                Thread.Sleep(motor.PollInterval);
            }
            motor.Brake();
            Thread.Sleep(motor.PollInterval*2);
            Console.WriteLine("fin wait {0}", motor.TachoCount - initTacho);
        }

        public static void RunWait(this NxtMotor motor, sbyte power, uint degrees)
        {
            motor.Poll();
            var initTacho = motor.TachoCount.Value;

            motor.Run(power, degrees);
            Thread.Sleep(motor.PollInterval*2);

            var lastTacho = motor.TachoCount.Value;
            for (int i = 0; i < 20; i++)
            {
                var tcount = motor.TachoCount.Value;
                if (tcount < initTacho+degrees/2 || lastTacho != tcount) {
                    i = 0;
                }
                lastTacho = tcount;
                Thread.Sleep(8);
            }

            Console.WriteLine("fin wait {0}", motor.TachoCount - initTacho);
        }
    }
}

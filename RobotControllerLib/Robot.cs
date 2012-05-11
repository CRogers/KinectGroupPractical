using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NKH.MindSqualls;
using Restrictor;

namespace RobotControllerLib
{
    public class Robot
    {
        public Arm LeftArm { get; private set; }
        public Arm RightArm { get; private set; }

        private NxtBrick[] nxtBricks;

        public JointMotorConfig JointMotorConfig { get; private set; }

        public Robot() : this(JointMotorConfig.Default) {}

        public Robot(JointMotorConfig jmc)
        {
            JointMotorConfig = jmc;

            // Make bluetooth connections - if it is -1 don't make connection
            nxtBricks = new NxtBrick[jmc.ComPorts.Length];
            var connectTasks = new Task<NxtBrick>[jmc.ComPorts.Length];

            for (int i = 0; i < jmc.ComPorts.Length; i++) {
                var comPort = jmc.ComPorts[i];
                if (comPort != -1) {
                    int j = i;
                    connectTasks[i] = new Task<NxtBrick>(() => ConnectBrick(j, comPort));
                    connectTasks[i].Start();
                }
            }

            Task.WaitAll(connectTasks);

            for (int i = 0; i < connectTasks.Length; i++) {
                nxtBricks[i] = connectTasks[i].Result;
            }

            // Add joints
            LeftArm =  new Arm(AddMotorToNxt(jmc.LeftShoulderAlong), AddMotorToNxt(jmc.LeftShoulderOut), AddMotorToNxt(jmc.LeftElbowAlong), AddMotorToNxt(jmc.LeftElbowOut));
            RightArm = new Arm(AddMotorToNxt(jmc.RightShoulderAlong), AddMotorToNxt(jmc.RightShoulderOut), AddMotorToNxt(jmc.RightElbowAlong), AddMotorToNxt(jmc.RightElbowOut));
        }

        private NxtBrick ConnectBrick(int brickNo, int comPort)
        {
            var brick = new NxtBrick(NxtCommLinkType.Bluetooth, (byte)comPort);
            var attempts = 3;
            for (int attempt = 0; attempt < attempts; attempt++) {
                Console.WriteLine("Connecting to brick {0} on port {1}. \t\tAttempt {2} of {3}...", brickNo, comPort, attempt + 1, attempts);
                try {
                    brick.Connect();
                    Console.WriteLine("\tBrick {0} successfully connected (port: {1})", brickNo, comPort);
                    break;
                } catch(IOException) {
                    if (attempt + 1 == attempts)
                        throw;
                } catch(AggregateException) {
                    if (attempt + 1 == attempts)
                        throw;
                }
            }

            return brick;
        }

        public void Disconnect()
        {
            // Stop all motors!
            Halt();

            foreach(var brick in nxtBricks)
                brick.Disconnect();
        }

        public void Halt()
        {
            LeftArm.ShoulderAlong.Halt();
            LeftArm.ShoulderOut.Halt();
            LeftArm.ElbowAlong.Halt();
            LeftArm.ElbowOut.Halt();

            RightArm.ShoulderAlong.Halt();
            RightArm.ShoulderOut.Halt();
            RightArm.ElbowAlong.Halt();
            RightArm.ElbowOut.Halt();
        }


        private NxtMotor AddMotorToNxt(MotorPort mp)
        {
            var motor = new NxtMotor();
            var brick = nxtBricks[mp.Brick];

            switch(mp.Motor) {
                case NxtMotorPort.PortA: brick.MotorA = motor; break;
                case NxtMotorPort.PortB: brick.MotorB = motor; break;
                case NxtMotorPort.PortC: brick.MotorC = motor; break;
            }

            return motor;
        }


        public void SetAngles(AnglePositions ap)
        {
            SetAngle(RightArm.ShoulderAlong, ap.RightShoulderAlong);
            SetAngle(RightArm.ShoulderOut, ap.RightShoulderOut);
            SetAngle(RightArm.ElbowAlong, ap.RightElbowAlong);
            SetAngle(RightArm.ElbowOut, ap.RightElbowOut);

            SetAngle(RightArm.ShoulderAlong, ap.RightShoulderAlong);
            SetAngle(RightArm.ShoulderOut, ap.RightShoulderOut);
            SetAngle(RightArm.ElbowAlong, ap.RightElbowAlong);
            SetAngle(RightArm.ElbowOut, ap.RightElbowOut);
        }

        private void SetAngle(Joint joint, int? angle)
        {
            if (angle.HasValue)
                LeftArm.ShoulderAlong.TargetAngle = angle.Value;
        }
    }
}

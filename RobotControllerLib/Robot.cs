using System;
using System.IO;
using System.Linq;
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
                if (comPort >= 0) {
                    int j = i; // Prevent access to modified closure
                    connectTasks[i] = new Task<NxtBrick>(() => ConnectBrick(j, jmc.LinkTypes[j], comPort));
                    connectTasks[i].Start();
                }
            }

            Task.WaitAll(connectTasks.Where(a => a != null).ToArray());

            Logger.Log("Connected all motors");

            for (int i = 0; i < connectTasks.Length; i++) {
                nxtBricks[i] = connectTasks[i] == null ? null : connectTasks[i].Result;
            }

            // Add joints
            LeftArm =  new Arm("Left", AddMotorToNxt(jmc.LeftShoulderAlong), AddMotorToNxt(jmc.LeftShoulderOut), AddMotorToNxt(jmc.LeftElbowAlong), AddMotorToNxt(jmc.LeftElbowOut));
            RightArm = new Arm("Right", AddMotorToNxt(jmc.RightShoulderAlong), AddMotorToNxt(jmc.RightShoulderOut), AddMotorToNxt(jmc.RightElbowAlong), AddMotorToNxt(jmc.RightElbowOut));
        }

        private NxtBrick ConnectBrick(int brickNo, NxtCommLinkType linkType, int comPort)
        {
            var brick = new NxtBrick(linkType, (byte)comPort);

            if (linkType == NxtCommLinkType.Bluetooth) {
                var attempts = 3;
                for (int attempt = 0; attempt < attempts; attempt++) {
                    Logger.Log("Connecting to brick {0} on port {1}. \t\tAttempt {2} of {3}...", brickNo, comPort,
                                      attempt + 1, attempts);
                    try {
                        brick.Connect();
                        Logger.Log("\tBrick {0} successfully connected (port: {1})", brickNo, comPort);
                        break;
                    }
                    catch (IOException) {
                        if (attempt + 1 == attempts)
                            throw;
                    }
                }
            }

            Logger.Log("Connected to brick {0}", brickNo);

            return brick;
        }

        public void Disconnect()
        {
            // Stop all motors!
            Halt();

            foreach(var brick in nxtBricks)
                if(brick != null)
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

            if (brick == null)
                return null;

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
                joint.TargetAngle = angle.Value;
        }
    }
}

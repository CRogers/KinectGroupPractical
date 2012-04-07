using System;
using NKH.MindSqualls;

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
            for(int i = 0; i < jmc.ComPorts.Length; i++) {
                var comPort = jmc.ComPorts[i];
                if(comPort != -1) {
                    var brick = new NxtBrick(NxtCommLinkType.Bluetooth, (byte)comPort);
                    nxtBricks[i] = brick;
                    brick.Connect();
                    Console.WriteLine("Connected to brick {0} on com port {1}", i, comPort);
                }
            }

            // Add joints
            LeftArm  = new Arm(GetMotorFromNxt("lShTw"), GetMotorFromNxt("lShEx"), GetMotorFromNxt("lElTw"), GetMotorFromNxt("lElEx"));
            RightArm = new Arm(GetMotorFromNxt("rShTw"), GetMotorFromNxt("rShEx"), GetMotorFromNxt("rElTw"), GetMotorFromNxt("rElEx"));
        }


        private NxtMotor GetMotorFromNxt(string joint)
        {
            var jointTuple = JointMotorConfig.JointMotorMapping[joint];
            var brick = nxtBricks[jointTuple.Item1];
            var port = jointTuple.Item2;

            switch(port) {
                case NxtMotorPort.PortA: return brick.MotorA;
                case NxtMotorPort.PortB: return brick.MotorB;
                case NxtMotorPort.PortC: return brick.MotorC;
                case null: return null;
            }

            throw new ArgumentException("Cannot return all the motor ports of brick");
        }
    }
}

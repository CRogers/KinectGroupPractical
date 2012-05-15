using System;
using System.Collections.Generic;
using NKH.MindSqualls;

namespace RobotControllerLib
{
    public class JointMotorConfig
    {
        public int[] ComPorts { get; private set; }
        public NxtCommLinkType[] LinkTypes { get; private set; }
        public Dictionary<string, Tuple<int, NxtMotorPort?>> JointMotorMapping { get; private set; }

        public MotorPort LeftShoulderAlong { get; set; }
        public MotorPort LeftShoulderOut { get; set; }
        public MotorPort LeftElbowAlong { get; set; }
        public MotorPort LeftElbowOut { get; set; }
        public MotorPort RightShoulderAlong { get; set; }
        public MotorPort RightShoulderOut { get; set; }
        public MotorPort RightElbowAlong { get; set; }
        public MotorPort RightElbowOut { get; set; }

        public JointMotorConfig(int comPort1, NxtCommLinkType link1, int comPort2, NxtCommLinkType link2, int comPort3, NxtCommLinkType link3)
        {
            ComPorts = new[] {comPort1, comPort2, comPort3};
            LinkTypes = new[] {link1, link2, link3};
        }

        // Bluetooth: 3, 9, 11. USB: 2, 4, 3
        public static JointMotorConfig Default = new JointMotorConfig(2, NxtCommLinkType.USB, 4, NxtCommLinkType.USB, 3, NxtCommLinkType.USB)
        {
            RightElbowAlong    = new MotorPort(NxtMotorPort.PortA, 0),
            LeftShoulderOut    = new MotorPort(NxtMotorPort.PortB, 0),
            LeftElbowAlong     = new MotorPort(NxtMotorPort.PortC, 0),

            LeftShoulderAlong  = new MotorPort(NxtMotorPort.PortB, 1),
            RightShoulderAlong = new MotorPort(NxtMotorPort.PortC, 1),

            RightElbowOut      = new MotorPort(NxtMotorPort.PortA, 2),
            RightShoulderOut   = new MotorPort(NxtMotorPort.PortB, 2),  
            LeftElbowOut       = new MotorPort(NxtMotorPort.PortC, 2),
        };
    }
}

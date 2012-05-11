using NKH.MindSqualls;

namespace RobotControllerLib
{
    public class Arm
    {
        public Joint ShoulderAlong { get; private set; }
        public Joint ShoulderOut { get; private set; }

        public Joint ElbowAlong { get; private set; }
        public Joint ElbowOut { get; private set; }

        public Arm(NxtMotor shoulderAlong, NxtMotor shoulderOut, NxtMotor elbowAlong, NxtMotor elbowOut)
        {
            ShoulderAlong = new Joint(shoulderAlong);
            ShoulderOut = new Joint(shoulderOut);
            ElbowAlong = new Joint(elbowAlong);
            ElbowOut = new Joint(elbowOut);
        }
    }
}

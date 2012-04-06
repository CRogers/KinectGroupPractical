using NKH.MindSqualls;

namespace RobotControllerLib
{
    class Arm
    {
        public Joint ShoulderTwist { get; private set; }
        public Joint ShoulderExtend { get; private set; }

        public Joint ElbowTwist { get; private set; }
        public Joint ElbowExtend { get; private set; }


        public Arm(NxtMotor shoulderTwistMotor, NxtMotor shoulderExtendMotor, NxtMotor elbowTwistMotor, NxtMotor elbowExtendMotor)
        {
            ShoulderTwist = new Joint(shoulderTwistMotor);
            ShoulderExtend = new Joint(shoulderExtendMotor);
            ElbowTwist = new Joint(elbowTwistMotor);
            ElbowExtend = new Joint(elbowExtendMotor);
        }
    }
}

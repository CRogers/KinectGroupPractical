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
            ShoulderAlong = new Joint(shoulderAlong) {DegreeScaleFactor = 24};
            ShoulderOut = new Joint(shoulderOut) {DegreeScaleFactor = 24};
            ElbowAlong = new Joint(elbowAlong) {Power = 10};
            ElbowOut = new Joint(elbowOut) {Power = 10};
        }
    }
}

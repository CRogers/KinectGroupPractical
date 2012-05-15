using NKH.MindSqualls;

namespace RobotControllerLib
{
    public class Arm
    {
        public string Name { get; private set; }

        public Joint ShoulderAlong { get; private set; }
        public Joint ShoulderOut { get; private set; }

        public Joint ElbowAlong { get; private set; }
        public Joint ElbowOut { get; private set; }

        public Arm(string name, NxtMotor shoulderAlong, NxtMotor shoulderOut, NxtMotor elbowAlong, NxtMotor elbowOut)
        {
            ShoulderAlong = new Joint(shoulderAlong, name+"ShoulderAlong") {DegreeScaleFactor = 24, MinAngle = -270, MaxAngle = 270};
            ShoulderOut = new Joint(shoulderOut, name + "ShoulderOut") { DegreeScaleFactor = 24, MinAngle = 0, MaxAngle = 200 };
            ElbowAlong = new Joint(elbowAlong, name + "ElbowAlong") { Power = 5, MinAngle = 0, MaxAngle = 115 };
            ElbowOut = new Joint(elbowOut, name + "ElbowOut") { Power = 5, MinAngle = -100, MaxAngle = 100 };
        }
    }
}

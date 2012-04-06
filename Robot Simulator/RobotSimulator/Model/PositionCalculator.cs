using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using RobotSimulator.Utility;

namespace RobotSimulator.Model
{
    //This class is designed to let you ask for the absolute coordinates of the robot's limbs.
    //It is necessary for collision checking. The limbs of the robot will need to be callibrated to match the real robot closely.

    /*
     * The methods for finding coordinates in this class work like this:
     * 
     * 1. If you supply null for an angle then the angle of the simulated robot is used as a default.
     * 
     * 2. If the method accepts a 3D Point, then using it will tell you where the centre of the relevant limb is + your point.
     *    For example, if I want to know where the centre of the head of the robot is, I would call
     *    
     *    getHeadCoords(null,null,new Point3D(0,0,0))
     *    
     *    If I knew the robot's head was of height 20, and I wanted to know where the top of the robot's head was, I would call:
     *    
     *    getHeadCoords(null,null,new Point3D(0,10,0))
     *    
     *    All body parts initially are centred about the origin. Hence, while this is so the top of the robot's head
     *    will appear at (0,10,0). The transformations then map this to the absolute position.
     * 
     * 3. If the method does NOT accept a 3D point, ATTOW (at the time of writing) these methods are for the shoulders and elbows,
     *    then please see them for what point they are telling you. There is a relative point method for every part of the robot,
     *    so these methods are just helper methods.
     *    
     * For example, if you wanted to find the absolute direction of the left lower arm, then simply find any two points on it by
     * specifying two different relative points to its centre. Techniques like this should be able to tell you anything you need.
     * 
     * When you call a method with a specified angle, it will not affect the simulated robot itself. The simulated robot's position
     * can only be modified through the motor manager. The matrices used here are somewhat separate therefore to the transformation
     * groups used by the robot, so you must be careful to update both when the robot changes. The matrices and the transformation
     * graph are constructed side by side in the Robot class, so it shouldn't be difficult.
     */ 
    class PositionCalculator
    {
        Robot robot;
        MotorManager motorManager;
        //Requires the robot and the motor manager so that it can choose default positions for the motor
        //and use the robot's transformation matrices.
        public PositionCalculator(Robot robot, MotorManager motorManager)
        {
            this.motorManager = motorManager;
            this.robot = robot;
        }

        //Return the coordinates around the base - (0,0,0) for centre.
        //base -> DONE
        public Point3D getBaseCoords(Point3D point)
        {
            return robot.baseTransform.Transform(point);
        }

        //Return the coordinates of the centre of the chest - (0,0,0) for centre.
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getChestCoords(double? chestMotorAngle, Point3D point)
        {
            if (chestMotorAngle == null)
            {
                chestMotorAngle = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.CHEST_MOTOR1));
            }
            Matrix3D chestRot = Transforms.makeAxisTransform(robot.chestMotorAxis(), robot.chestAdjust(chestMotorAngle.GetValueOrDefault())).Value;
            return getBaseCoords(chestRot.Transform(robot.chestTransform.Transform(point)));
        }

        //Return the coordinates of the neck - (0,0,0) for centre
        //NECKMOTOR -> neck -> CHESTMOTOR -> chest -> base -> DONE
        public Point3D getNeckCoords(double? neckMotorAngle, double? chestMotorAngle, Point3D point)
        {
            if (neckMotorAngle==null)
            {
                neckMotorAngle = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.NECK_MOTOR1));
            }
            Matrix3D neckRot = Transforms.makeAxisTransform(robot.neckMotorAxis(), robot.neckAdjust(neckMotorAngle.GetValueOrDefault())).Value;
            return getChestCoords(chestMotorAngle,neckRot.Transform(robot.neckTransform.Transform(point)));
        }

        //Return the coordinates of the centre of the head - (0,0,0) for centre
        //head -> NECKMOTOR -> neck -> CHESTMOTOR -> chest -> base -> DONE
        public Point3D getHeadCoords(double? neckMotorAngle, double? chestMotorAngle, Point3D point)
        {
            return getNeckCoords(neckMotorAngle, chestMotorAngle, robot.headTransform.Transform(point));
        }

        //Return the coordinates of the centre of the left arm joint - (0,0,0) for centre
        //leftArmJoin -> CHESTMOTOR -> chest -> base -> DONE
        public Point3D getLeftArmJoinCoords(double? chestMotorAngle, Point3D point)
        {
            return getChestCoords(chestMotorAngle,robot.leftArmJoinTransform.Transform(point));
            //return chestRot.Transform(robot.leftArmJoinTransform.Transform(point));
        }

        //Return the coordinates of the left shoulder, which is where
        //the left upper arm is rotated about. If you wish to find a point relative to the upper arm, then use
        //the getLeftUpperArmCoords (ATTOW) instead.
        //leftShoulder -> leftArmJoin -> CHESTMOTOR -> chest -> base -> DONE
        public Point3D getLeftShoulderCentreCoords(double? chestMotorAngle)
        {
            return getLeftArmJoinCoords(chestMotorAngle, new Point3D(0, 0, 0));
        }

        //The function itself:
        private Point3D getLeftShoulderCentreCoords(double? chestMotorAngle, Point3D point)
        {
            return getLeftArmJoinCoords(chestMotorAngle,robot.leftShoulderTransform.Transform(point));
        }

        //Return the coordinates of the centre of the left upper arm - (0,0,0) for centre
        //leftUpperArm -> LEFTUPPERMOTOR1 -> LEFTUPPERMOTOR2 -> leftShoulder -> leftArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getLeftUpperArmCoords(double? leftShoulderMotorAngle1,
            double? leftShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            if (leftShoulderMotorAngle1 == null)
            {
                leftShoulderMotorAngle1 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR1));
            }
            if (leftShoulderMotorAngle2 == null)
            {
                leftShoulderMotorAngle2 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR2));
            }
            Matrix3D leftShoulderRot1 = Transforms.makeAxisTransform(robot.leftShoulderMotor1Axis(), robot.leftShoulder1Adjust(leftShoulderMotorAngle1.GetValueOrDefault())).Value;
            Matrix3D leftShoulderRot2 = Transforms.makeAxisTransform(robot.leftShoulderMotor2Axis(leftShoulderMotorAngle1.GetValueOrDefault()), robot.leftShoulder2Adjust(leftShoulderMotorAngle2.GetValueOrDefault())).Value;
            return getLeftShoulderCentreCoords(chestMotorAngle, leftShoulderRot2.Transform(leftShoulderRot1.Transform(robot.leftUpperArmTransform.Transform(point))));
        }

        //Return the coordinates of the left elbow, which is where the left lower arm is rotated about.
        //leftElbow -> leftUpperArm -> LEFTUPPERMOTOR1 -> LEFTUPPERMOTOR2 -> leftShoulder -> leftArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getLeftElbowCentreCoords(double? leftShoulderMotorAngle1,
            double? leftShoulderMotorAngle2, double? chestMotorAngle)
        {
            return getLeftElbowCentreCoords(leftShoulderMotorAngle1, leftShoulderMotorAngle2, chestMotorAngle, new Point3D(0,0,0));
        }

        //The actual function:
        public Point3D getLeftElbowCentreCoords(double? leftShoulderMotorAngle1,
            double? leftShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            return getLeftUpperArmCoords(leftShoulderMotorAngle1,leftShoulderMotorAngle2,chestMotorAngle,robot.leftElbowTransform.Transform(point));
        }

        //Return the coordinates of the centre of the left lower arm - (0,0,0) for centre
        //leftLowerArm -> LEFTELBOWMOTOR1 -> LEFTELBOWMOTOR2 -> leftElbow -> leftUpperArm ->
        //LEFTUPPERMOTOR1 -> LEFTUPPERMOTOR2 -> leftArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getLeftLowerArmCoords(double? leftElbowMotorAngle1,
            double? leftElbowMotorAngle2, double? leftShoulderMotorAngle1,
            double? leftShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            if (leftElbowMotorAngle1 == null)
            {
                leftElbowMotorAngle1 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR1));
            }
            if (leftElbowMotorAngle2 == null)
            {
                leftElbowMotorAngle2 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR2));
            }
            Matrix3D leftElbowRot1 = Transforms.makeAxisTransform(robot.leftElbowMotor1Axis(), robot.leftElbow1Adjust(leftElbowMotorAngle1.GetValueOrDefault())).Value;
            Matrix3D leftElbowRot2 = Transforms.makeAxisTransform(robot.leftElbowMotor2Axis(leftElbowMotorAngle1.GetValueOrDefault()), robot.leftElbow2Adjust(leftElbowMotorAngle2.GetValueOrDefault())).Value;
            return getLeftElbowCentreCoords(leftShoulderMotorAngle1, leftShoulderMotorAngle2, chestMotorAngle,
                leftElbowRot2.Transform(leftElbowRot1.Transform(robot.leftLowerArmTransform.Transform(point))));
        }

        //Return the coordinates of the centre of the left hand - (0,0,0) for centre
        //leftHand -> leftLowerArm -> LEFTELBOWMOTOR1 -> LEFTELBOWMOTOR2 -> leftElbow -> leftUpperArm ->
        //LEFTUPPERMOTOR1 -> LEFTUPPERMOTOR2 -> leftArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getLeftHandCentreCoord(double? leftElbowMotorAngle1,
            double? leftElbowMotorAngle2, double? leftShoulderMotorAngle1,
            double? leftShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            return getLeftLowerArmCoords(leftElbowMotorAngle1,
            leftElbowMotorAngle2, leftShoulderMotorAngle1,
            leftShoulderMotorAngle2, chestMotorAngle,robot.leftHandTransform.Transform(point));
        }

        //****************************************** other arm ***************************************

        //Return the coordinates of the centre of the right arm joint - (0,0,0) for centre
        //rightArmJoin -> CHESTMOTOR -> chest -> base -> DONE
        public Point3D getRightArmJoinCoords(double? chestMotorAngle, Point3D point)
        {
            return getChestCoords(chestMotorAngle, robot.rightArmJoinTransform.Transform(point));
        }

        //Return the coordinates of the right shoulder, which is where
        //the right upper arm is rotated about.
        //rightShoulder -> rightArmJoin -> CHESTMOTOR -> chest -> base -> DONE
        public Point3D getRightShoulderCentreCoords(double? chestMotorAngle)
        {
            return getRightArmJoinCoords(chestMotorAngle, new Point3D(0, 0, 0));
        }

        //The function itself:
        private Point3D getRightShoulderCentreCoords(double? chestMotorAngle, Point3D point)
        {
            return getRightArmJoinCoords(chestMotorAngle, robot.rightShoulderTransform.Transform(point));
        }

        //Return the coordinates of the centre of the right upper arm - (0,0,0) for centre
        //rightUpperArm -> RIGHTUPPERMOTOR1 -> RIGHTUPPERMOTOR2 -> rightShoulder -> rightArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getRightUpperArmCoords(double? rightShoulderMotorAngle1,
            double? rightShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            if (rightShoulderMotorAngle1 == null)
            {
                rightShoulderMotorAngle1 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR1));
            }
            if (rightShoulderMotorAngle2 == null)
            {
                rightShoulderMotorAngle2 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR2));
            }
            Matrix3D rightShoulderRot1 = Transforms.makeAxisTransform(robot.rightShoulderMotor1Axis(), robot.rightShoulder1Adjust(rightShoulderMotorAngle1.GetValueOrDefault())).Value;
            Matrix3D rightShoulderRot2 = Transforms.makeAxisTransform(robot.rightShoulderMotor2Axis(rightShoulderMotorAngle1.GetValueOrDefault()), robot.rightShoulder2Adjust(rightShoulderMotorAngle2.GetValueOrDefault())).Value;
            return getRightShoulderCentreCoords(chestMotorAngle, rightShoulderRot2.Transform(rightShoulderRot1.Transform(robot.rightUpperArmTransform.Transform(point))));
        }

        //Return the coordinates of the right elbow, which is where the right lower arm is rotated about
        //rightElbow -> rightUpperArm -> RIGHTUPPERMOTOR1 -> RIGHTUPPERMOTOR2 -> rightShoulder -> rightArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getRightElbowCentreCoords(double? rightShoulderMotorAngle1,
            double? rightShoulderMotorAngle2, double? chestMotorAngle)
        {
            return getRightElbowCentreCoords(rightShoulderMotorAngle1, rightShoulderMotorAngle2, chestMotorAngle, new Point3D(0, 0, 0));
        }

        //The actual function:
        public Point3D getRightElbowCentreCoords(double? rightShoulderMotorAngle1,
            double? rightShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            return getRightUpperArmCoords(rightShoulderMotorAngle1, rightShoulderMotorAngle2, chestMotorAngle, robot.rightElbowTransform.Transform(point));
        }

        //Return the coordinates of the centre of the right lower arm - (0,0,0) for centre
        //rightLowerArm -> RIGHTELBOWMOTOR1 -> RIGHTELBOWMOTOR2 -> rightElbow -> rightUpperArm ->
        //RIGHTUPPERMOTOR1 -> RIGHTUPPERMOTOR2 -> rightArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getRightLowerArmCoords(double? rightElbowMotorAngle1,
            double? rightElbowMotorAngle2, double? rightShoulderMotorAngle1,
            double? rightShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            if (rightElbowMotorAngle1 == null)
            {
                rightElbowMotorAngle1 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR1));
            }
            if (rightElbowMotorAngle2 == null)
            {
                rightElbowMotorAngle2 = MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR2));
            }
            Matrix3D rightElbowRot1 = Transforms.makeAxisTransform(robot.rightElbowMotor1Axis(), robot.rightElbow1Adjust(rightElbowMotorAngle1.GetValueOrDefault())).Value;
            Matrix3D rightElbowRot2 = Transforms.makeAxisTransform(robot.rightElbowMotor2Axis(rightElbowMotorAngle1.GetValueOrDefault()), robot.rightElbow2Adjust(rightElbowMotorAngle2.GetValueOrDefault())).Value;
            return getRightElbowCentreCoords(rightShoulderMotorAngle1, rightShoulderMotorAngle2, chestMotorAngle,
                rightElbowRot2.Transform(rightElbowRot1.Transform(robot.rightLowerArmTransform.Transform(point))));
        }

        //Return the coordinates of the centre of the right hand - (0,0,0) for centre
        //rightHand -> rightLowerArm -> RIGHTELBOWMOTOR1 -> RIGHTELBOWMOTOR2 -> rightElbow -> rightUpperArm ->
        //RIGHTUPPERMOTOR1 -> RIGHTUPPERMOTOR2 -> rightArmJoin ->
        //CHESTMOTOR -> chest -> base -> DONE
        public Point3D getRightHandCentreCoord(double? rightElbowMotorAngle1,
            double? rightElbowMotorAngle2, double? rightShoulderMotorAngle1,
            double? rightShoulderMotorAngle2, double? chestMotorAngle, Point3D point)
        {
            return getRightLowerArmCoords(rightElbowMotorAngle1,
            rightElbowMotorAngle2, rightShoulderMotorAngle1,
            rightShoulderMotorAngle2, chestMotorAngle, robot.rightHandTransform.Transform(point));
        }
    }
}

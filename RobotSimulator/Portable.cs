using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Model;
using RobotSimulator.View;
using RobotSimulator.Controller;
using System.Windows.Controls;
using RobotSimulator.Utility;
using Restrictor;

namespace RobotSimulator
{
    /*
     * This class provides a simplified version of the robot simulator designed to enable you to more easily
     * import the robot without actually displaying it. It provides a few methods that allow you to return the motor
     * manager and position calculator automatically.
     * You may optionally pass in a ViewPort3D if you wish for the robot to be displayed somewhere.
     */
    public class Portable : I_MotorListener
    {

        private MotorManager motorManager;
        private PositionCalculator positionCalculator;
        private ViewPlatform platform;
        private Robot robot;
        public const bool INCREASING = true, DECREASING = false;
        /// <summary>
        /// Create a new Portable object. This class is designed specifically to be imported by other solutions. By calling this constructor,
        /// the class will provide you with a robot which does not appear on a display, but otherwise works in the same way.
        /// You can set motor degrees through this class in the normal 360 fashion too.
        /// </summary>
        public Portable()
        {
            //Now initialise the display!
            motorManager = new MotorManager();
            robot = new Robot();
            robot.intialise(new Viewport3D());
            //Add myself as a listener for the motor manager:
            motorManager.addListener(this);
            platform = new ViewPlatform(new Viewport3D(), motorManager, robot);
            positionCalculator = new PositionCalculator(robot, motorManager);
            updateMotors();
        }

        /// <summary>
        /// Create a new Portable object. This class is designed specifically to be imported by other solutions. By calling this constructor,
        /// the class will provide you with a robot which appears on the screen. Calling the methods in this class will change the appearance
        /// of the robot on screen as well. You can use methods for setting degrees in this class with the normal 360 fashion too.
        /// Note: you must call "setCameraToListenToKeys" to be able to move the camera with the keyboard. Otherwise the camera
        /// can't know when to move.
        /// </summary>
        /// <param name="viewport">The viewport to use for displaying the robot.</param>
        public Portable(Viewport3D viewport)
        {
            //Now initialise the display!
            motorManager = new MotorManager();
            robot = new Robot();
            platform = new ViewPlatform(viewport, motorManager, robot);
            positionCalculator = new PositionCalculator(robot, motorManager);
            updateMotors();
        }

        /// <summary>
        /// Tell the camera which displays the robot to listen to a particular key handler. The camera will expect AWSDEFZX command
        /// keys to control its movement at the time of writing (see the ViewPlatform class for details). This is only worth
        /// doing if you provided a viewport initially.
        /// </summary>
        /// <param name="keyHandler">The key handler that the camera should listen to.</param>
        public void setCameraToListenToKeys(I_Observable<I_KeyListener> keyHandler)
        {
            if (keyHandler == null) throw new ArgumentException("Unexpected null key handler.");
            keyHandler.addListener(platform);
        }

        /// <summary>
        /// Return the angle position object represented by the dummy robot's current position.
        /// (So thid does the conversion for you)
        /// </summary>
        /// <returns>The angle positions object</returns>
        public AnglePositions getAngles()
        {
            AnglePositions angles = new AnglePositions();
            angles.LeftShoulderAlong = (int)getMotor(MotorManager.LEFT_SHOULDER_MOTOR1);
            angles.LeftShoulderOut = (int)getMotor(MotorManager.LEFT_SHOULDER_MOTOR2);
            angles.LeftElbowAlong = (int)getMotor(MotorManager.LEFT_ELBOW_MOTOR1);
            angles.LeftElbowOut = (int)getMotor(MotorManager.LEFT_ELBOW_MOTOR2);
            angles.RightShoulderAlong = (int)getMotor(MotorManager.RIGHT_SHOULDER_MOTOR1);
            angles.RightShoulderOut = (int)getMotor(MotorManager.RIGHT_SHOULDER_MOTOR2);
            angles.RightElbowAlong = (int)getMotor(MotorManager.RIGHT_ELBOW_MOTOR1);
            angles.RightElbowOut = (int)getMotor(MotorManager.RIGHT_ELBOW_MOTOR2);
            return angles;
        }

        /// <summary>
        /// Set the angles of the robot using an AnglesPosition object instead
        /// </summary>
        /// <param name="angles">The angles object you wish to use (null values will be ignored)</param>
        public void setAngles(AnglePositions angles)
        {
            if (angles.LeftShoulderAlong != null) setMotor(MotorManager.LEFT_SHOULDER_MOTOR1, angles.LeftShoulderAlong.Value);
            if (angles.LeftShoulderOut != null) setMotor(MotorManager.LEFT_SHOULDER_MOTOR2, angles.LeftShoulderOut.Value);
            if (angles.LeftElbowAlong != null) setMotor(MotorManager.LEFT_ELBOW_MOTOR1, angles.LeftElbowAlong.Value);
            if (angles.LeftElbowOut != null) setMotor(MotorManager.LEFT_ELBOW_MOTOR2, angles.LeftElbowOut.Value);

            if (angles.RightShoulderAlong != null) setMotor(MotorManager.RIGHT_SHOULDER_MOTOR1, angles.RightShoulderAlong.Value);
            if (angles.RightShoulderOut != null) setMotor(MotorManager.RIGHT_SHOULDER_MOTOR2, angles.RightShoulderOut.Value);
            if (angles.RightElbowAlong != null) setMotor(MotorManager.RIGHT_ELBOW_MOTOR1, angles.RightElbowAlong.Value);
            if (angles.RightElbowOut != null) setMotor(MotorManager.RIGHT_ELBOW_MOTOR2, angles.RightElbowOut.Value);
        }

        /// <summary>
        /// Retrieve the motor manager being used by the application.
        /// Note: the Portable class provides additional methods for setting the motors, and it is recommended that you use these.
        /// </summary>
        /// <returns>The motor manager for the robot.</returns>
        public MotorManager motors()
        {
            return motorManager;
        }

        /// <summary>
        /// Return the position calculator being used by the application.
        /// </summary>
        /// <returns>The position calculator for the virtual robot.</returns>
        public PositionCalculator positions()
        {
            return positionCalculator;
        }

        //Constants for specifing different components:
        public const byte LEFTHAND = 0, LEFTARMJOIN = 1, LEFTLOWERARM = 2, LEFTUPPERARM = 3;
        public const byte RIGHTHAND = 4, RIGHTARMJOIN = 5, RIGHTLOWERARM = 6, RIGHTUPPERARM = 7;
        public const byte HEAD = 8, NECK = 9, CHEST = 10, BASE = 11;
        //For specifying the specific dimension of the piece:
        //(depth z axis, height y axis, width x axis).
        //To work out which direction is which, consider the robot in the standing position.
        public const byte WIDTH = 0, HEIGHT = 1, DEPTH = 2;

        /// <summary>
        /// This method can tell you the dimensions of any of the robot's body parts.
        /// These dimensions correspond to the robot in the standing position. For example, the width of the arm
        /// is the horizontal length of it when the robot is standing still (in the x direction).
        /// </summary>
        /// <param name="bodypart">Which part of the robot you are interested. (Use Portable's constants)</param>
        /// <param name="axis">Which axis you are interested in. (Use Portable's constants)</param>
        /// <returns>The length of the specified part+direction</returns>
        public double getSize(byte bodypart, byte axis)
        {
            Robot.Cuboid cuboid;
            switch (bodypart)
            {
                //Check every case...
                case LEFTHAND: cuboid = robot.LeftHand; break;
                case LEFTARMJOIN: cuboid = robot.LeftArmJoin; break;
                case LEFTLOWERARM: cuboid = robot.LeftLowerArm; break;
                case LEFTUPPERARM: cuboid = robot.LeftUpperArm; break;
                case RIGHTHAND: cuboid = robot.RightHand; break;
                case RIGHTARMJOIN: cuboid = robot.RightArmJoin; break;
                case RIGHTLOWERARM: cuboid = robot.RightLowerArm; break;
                case RIGHTUPPERARM: cuboid = robot.RightUpperArm; break;
                case HEAD: cuboid = robot.Head; break;
                case NECK: cuboid = robot.Neck; break;
                case BASE: cuboid = robot.Base; break;
                case CHEST: cuboid = robot.Chest; break;
                default: throw new ArgumentException("Unrecognised body part with number: " + bodypart.ToString());
            }
            switch (axis)
            {
                //Return the correct length
                case WIDTH: return cuboid.XLength;
                case HEIGHT: return cuboid.YLength;
                case DEPTH: return cuboid.ZLength;
                default: throw new ArgumentException("Unrecognised desired axis with number: " + axis.ToString());
            }
        }

        /// <summary>
        /// Return the angle of the motor as it is right now.
        /// </summary>
        /// <param name="motor">The motor you are interested in. (Use the constants from MotorManager)</param>
        /// <returns></returns>
        public double getMotor(int motor)
        {
            return MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(motor));
        }

        /// <summary>
        /// Set the motor to a fixed specified angle. This will cancel all timers and force the motor to that
        /// angle instantly.
        /// </summary>
        /// <param name="motor">The motor to set. (Use the constants from MotorManager)</param>
        /// <param name="angle">The angle in degrees (0-360) to set the motor too.</param>
        public void setMotor(int motor, double angle)
        {
            motorManager.setMotorDegrees(motor, MoreMaths.toMotorAngle(angle));
        }
        /// <summary>
        /// Set the motor to move towards an angle with a given speed.
        /// </summary>
        /// <param name="motor">The motor to set. (Use the constants from MotorManager)</param>
        /// <param name="angle">The angle to move towards in degrees (0-360)</param>
        /// <param name="direction">The direction to the motor should move in. That is, should the angle
        /// increase towards its target or decrease?</param>
        /// <param name="stepTime">How regularly the motor takes a step towards its target in milliseconds.</param>
        /// <param name="stepDist">This should be a positive number indicating the change in degrees at each step.</param>
        public void setMotorWithSpeed(int motor, double angle, bool direction, double stepTime, double stepDist)
        {
            if (stepDist <= 0) throw new ArgumentOutOfRangeException("Step distance must be positive. It had value: " + stepDist.ToString());
            if (direction == INCREASING)
            {
                motorManager.setMotorDegrees(motor, MoreMaths.toMotorAngle(angle), MoreMaths.toMotorAngle(stepDist), stepTime);
            }
            else
            {
                motorManager.setMotorDegrees(motor, MoreMaths.toMotorAngle(angle), MoreMaths.toMotorAngle(stepDist) * -1, stepTime);
            }
        }

        /// <summary>
        /// Set the motor to a fixed specified angle added on to the current angle (relative).
        /// This will cancel all timers and force the motor to that angle instantly.
        /// </summary>
        /// <param name="motor">The motor to set. (Use the constants from MotorManager)</param>
        /// <param name="angle">The angle in degrees (0-360) to set the motor too relative to its current angle.</param>
        public void setMotorRelative(int motor, double angle)
        {
            setMotor(motor, MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(motor)) + angle);
        }

        /// <summary>
        /// Set the motor to move towards a relative angle (added on to the existing angle) with a given speed.
        /// </summary>
        /// <param name="motor">The motor to set. (Use the constants from MotorManager)</param>
        /// <param name="angle">The angle to move towards in degrees (0-360)</param>
        /// <param name="direction">The direction to the motor should move in. That is, should the angle
        /// increase towards its target or decrease?</param>
        /// <param name="stepTime">How regularly the motor takes a step towards its target in milliseconds.</param>
        /// <param name="stepDist">This should be a positive number indicating the change in degrees at each step.</param>
        public void setMotorWithSpeedRelative(int motor, double angle, bool direction, double stepTime, double stepDist)
        {
            setMotorWithSpeed(motor, MoreMaths.fromMotorAngle(motorManager.getMotorDegrees(motor)) + angle, direction, stepTime, stepDist);
        }

        //Implementing the required interface
        public void motorSetTo(int motor, double degrees)
        {
            //Don't care
        }

        public void motorMovedTo(int motor, double degrees)
        {
            //Update the motor position on the robot.
            //The incoming value is in degrees from 0 to 255 (roughly), so adjust to
            //360 degrees first:
            degrees = Utility.MoreMaths.fromMotorAngle(degrees);
            //Now set the motor:
            switch (motor)
            {
                case MotorManager.CHEST_MOTOR1:
                    robot.ChestMotor1 = degrees;
                    break;
                case MotorManager.LEFT_ELBOW_MOTOR1:
                    robot.LeftElbowMotor1 = degrees;
                    break;
                case MotorManager.LEFT_ELBOW_MOTOR2:
                    robot.LeftElbowMotor2 = degrees;
                    break;
                case MotorManager.LEFT_SHOULDER_MOTOR1:
                    robot.LeftShoulderMotor1 = degrees;
                    break;
                case MotorManager.LEFT_SHOULDER_MOTOR2:
                    robot.LeftShoulderMotor2 = degrees;
                    break;
                case MotorManager.NECK_MOTOR1:
                    robot.NeckMotor1 = degrees;
                    break;
                case MotorManager.RIGHT_ELBOW_MOTOR1:
                    robot.RightElbowMotor1 = degrees;
                    break;
                case MotorManager.RIGHT_ELBOW_MOTOR2:
                    robot.RightElbowMotor2 = degrees;
                    break;
                case MotorManager.RIGHT_SHOULDER_MOTOR1:
                    robot.RightShoulderMotor1 = degrees;
                    break;
                case MotorManager.RIGHT_SHOULDER_MOTOR2:
                    robot.RightShoulderMotor2 = degrees;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unrecognised motor from motor manager.", "motor");
            }
        }

        public void motorReached(int motor, double degrees)
        {
            //Don't care
        }

        //Sync the motor positions with the motor manager:
        private void updateMotors()
        {
            robot.ChestMotor1 = (motorManager.getMotorDegrees(MotorManager.CHEST_MOTOR1));
            robot.LeftElbowMotor1 = (motorManager.getMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR1));
            robot.LeftElbowMotor2 = (motorManager.getMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR2));
            robot.LeftShoulderMotor1 = (motorManager.getMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR1));
            robot.LeftShoulderMotor2 = (motorManager.getMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR2));
            robot.NeckMotor1 = (motorManager.getMotorDegrees(MotorManager.NECK_MOTOR1));
            robot.RightElbowMotor1 = (motorManager.getMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR1));
            robot.RightElbowMotor2 = (motorManager.getMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR2));
            robot.RightShoulderMotor1 = (motorManager.getMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR1));
            robot.RightShoulderMotor2 = (motorManager.getMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR2));
        }
    }
}
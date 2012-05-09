using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Model;
using RobotSimulator.View;
using RobotSimulator.Controller;
using System.Windows.Controls;
using RobotSimulator.Utility;

namespace RobotSimulator
{
    /*
     * This class provides a simplified version of the robot simulator designed to enable you to more easily
     * import the robot without actually displaying it. It provides a few methods that allow you to return the motor
     * manager and position calculator automatically.
     * You may optionally pass in a ViewPort3D if you wish for the robot to be displayed somewhere.
     */
    class Portable : I_MotorListener
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
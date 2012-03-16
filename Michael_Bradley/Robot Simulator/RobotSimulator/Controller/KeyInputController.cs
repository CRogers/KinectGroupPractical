using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Utility;
using RobotSimulator.Model;
using System.Windows.Input;

namespace RobotSimulator.Controller
{
    //This controller interprets certain key inputs from the user and turns them into
    //commands to move the motors. This is an example class of how other projects can interacts
    //with the virtual motors. The motors are over parameterised intentionally so that
    //when the robot is actually built they can be callibrated if necessary to match it.
    class KeyInputController : I_KeyListener
    {
        private MotorManager motorManager;
        public KeyInputController(MotorManager motorManager)
        {
            this.motorManager = motorManager;
            //You can attach yourself to listen to the motors:
            //motorManager.addListener(this);
        }
        public void KeyPressed(System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0: //the neck motor
                    //Tell the motor manager to change motor instantaneously:
                    motorManager.setMotorDegrees(MotorManager.NECK_MOTOR1, motorManager.getMotorDegrees(MotorManager.NECK_MOTOR1) + (255.0 / 4.0), 0.8, 10);
                    break;
                case Key.D1: //the chest motor
                    motorManager.setMotorDegrees(MotorManager.CHEST_MOTOR1, motorManager.getMotorDegrees(MotorManager.CHEST_MOTOR1) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D2: //the left shoulder motor 1
                    motorManager.setMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR1, motorManager.getMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR1) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D3: //the left shoulder motor 2
                    motorManager.setMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR2, motorManager.getMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR2) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D4: //the right shoulder motor 1
                    motorManager.setMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR1, motorManager.getMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR1) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D5: //the right shoulder motor 2
                    motorManager.setMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR2, motorManager.getMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR2) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D6: //the left elbow motor 1
                    motorManager.setMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR1, motorManager.getMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR1) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D7: //the left elbow motor 2
                    motorManager.setMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR2, motorManager.getMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR2) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D8: //the right elbow motor 1
                    motorManager.setMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR1, motorManager.getMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR1) + (255 / 4.0), 0.8, 10);
                    break;
                case Key.D9: //the right elbow motor 2
                    motorManager.setMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR2, motorManager.getMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR2) + (255 / 4.0), 0.8, 10);
                    break;
                default:
                    break;
            }
        }

        public void KeyReleased(System.Windows.Input.KeyEventArgs e)
        {
            //don't need to know
        }
    }
}

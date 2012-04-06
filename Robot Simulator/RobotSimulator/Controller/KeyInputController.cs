using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Utility;
using RobotSimulator.Model;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace RobotSimulator.Controller
{
    //This controller interprets certain key inputs from the user and turns them into
    //commands to move the motors. This is an example class of how other projects can interacts
    //with the virtual motors. The motors are over parameterised intentionally so that
    //when the robot is actually built they can be callibrated if necessary to match it.
    class KeyInputController : I_KeyListener
    {
        private MotorManager motorManager;
        private PositionCalculator positionCalculator;
        public KeyInputController(MotorManager motorManager, PositionCalculator positionCalculator)
        {
            this.positionCalculator = positionCalculator;
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
                    motorManager.setMotorDegrees(MotorManager.NECK_MOTOR1, motorManager.getMotorDegrees(MotorManager.NECK_MOTOR1) + (255 / 4.0), 0.8, 10);
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
                case Key.H: //return the coordinates of the head
                    System.Windows.MessageBox.Show("Head");
                    System.Windows.MessageBox.Show(positionCalculator.getHeadCoords(null, null, new Point3D(0,0,0)).ToString());
                    break;
                case Key.C: //return the coordinates of the chest
                    System.Windows.MessageBox.Show("Chest");
                    System.Windows.MessageBox.Show(positionCalculator.getChestCoords(null,new Point3D(0,0,0)).ToString());
                    break;
                case Key.B: //return the coordinates of the base
                    System.Windows.MessageBox.Show("Base");
                    System.Windows.MessageBox.Show(positionCalculator.getBaseCoords(new Point3D(0,0,0)).ToString());
                    break;
                case Key.N: //return the coordinates of the left arm joint
                    System.Windows.MessageBox.Show("Left Arm Joint");
                    System.Windows.MessageBox.Show(positionCalculator.getLeftArmJoinCoords(null, new Point3D(0, 0, 0)).ToString());
                    break;
                case Key.M: //return the coordinates of the left upper arm
                    System.Windows.MessageBox.Show("Left Upper Arm");
                    System.Windows.MessageBox.Show(positionCalculator.getLeftUpperArmCoords(null,null,null,new Point3D(0,0,0)).ToString());
                    break;
                case Key.J: //return the coordinates of the left lower arm
                    System.Windows.MessageBox.Show("Left Lower Arm");
                    System.Windows.MessageBox.Show(positionCalculator.getLeftLowerArmCoords(null,null,null,null,null,new Point3D(0,0,0)).ToString());
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

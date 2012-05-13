using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using RobotSimulator.View;
using RobotSimulator.Model;
using RobotSimulator.Controller;
using RobotSimulator.Utility;

namespace RobotSimulator
{
    //This class links the classes in the solution together to enable the robot to function!
    class Linker
    {
        //Create a linker without hooking it up to any key input (this will not let you move the camera,
        //so it is not advised).

        MotorManager motorManager;
        PositionCalculator positionCalculator;
        KeyInputController keyController;
        ViewPlatform platform;
        public Linker(Viewport3D viewport)
        {
            //Now initialise the display!
            motorManager = new MotorManager();
            Robot robot = new Robot();
            platform = new ViewPlatform(viewport, motorManager, robot);
            positionCalculator = new PositionCalculator(robot, motorManager);
            keyController = new KeyInputController(motorManager, positionCalculator);
        }

        //Create a linker with a key handler, enabling you to move the camera with the WASDEFZX keys.
        //If you pass null, it will not link the keys.
        public Linker(I_Observable<I_KeyListener> keyHandler,Viewport3D viewport) : this(viewport)
        {
            if (keyHandler != null)
            {
                keyHandler.addListener(keyController);
                keyHandler.addListener(platform); //let the view port listen to my key events
            }
        }

        public void updateMotors(AngleSet angles)
        {
            motorManager.setMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR1, (angles.rightShoulder1 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.RIGHT_SHOULDER_MOTOR2, (angles.rightShoulder2 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR1, (angles.rightElbow1 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.RIGHT_ELBOW_MOTOR2, (angles.rightElbow2 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR1, (angles.leftShoulder1 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.LEFT_SHOULDER_MOTOR2, (angles.leftShoulder2 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR1, (angles.leftElbow1 / 360) * 255);
            motorManager.setMotorDegrees(MotorManager.LEFT_ELBOW_MOTOR2, (angles.leftElbow2 / 360) * 255);

        }
    }
}

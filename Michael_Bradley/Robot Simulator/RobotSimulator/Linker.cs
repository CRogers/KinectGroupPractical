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
        public Linker(Viewport3D viewport)
        {
            //Now initialise the display!
            MotorManager motorManager = new MotorManager();
            ViewPlatform platform = new ViewPlatform(viewport, motorManager);
            KeyInputController keyController = new KeyInputController(motorManager);
        }

        //Create a linker with a key handler, enabling you to move the camera with the WASDEFZX keys.
        //If you pass null, it will not link the keys.
        public Linker(I_Observable<I_KeyListener> keyHandler,Viewport3D viewport)
        {
            //Now initialise the display!
            MotorManager motorManager = new MotorManager();
            ViewPlatform platform = new ViewPlatform(viewport, motorManager);
            KeyInputController keyController = new KeyInputController(motorManager);
            if (keyHandler != null)
            {
                keyHandler.addListener(keyController);
                keyHandler.addListener(platform); //let the view port listen to my key events
            }
        }
    }
}

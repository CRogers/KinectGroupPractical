using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Model;
using System.Windows.Controls;
using RobotSimulator;
using RobotSimulator.Utility;
using System.Timers;

namespace Restrictor
{
    public class CollisionRestrictor : I_Observable<I_AngleListener>
    {
        public static CollisionRestrictor INSTANCE = new CollisionRestrictor();
        //The listeners for the motor events:
        private ICollection<I_AngleListener> listeners;
        private Timer timer; //the timers for each motor
        //The two robots that are used in by the algorithms.
        Portable fictionalRobot;
        Portable kinectRobot;
        AnglePositions realAngles;
        private bool robotReady = false;
        private RobotControllerLib.Robot robot;
        private CollisionRestrictor()
        {
            listeners = new LinkedList<I_AngleListener>();
        }
        public void intialise()
        {
            initialise(new Viewport3D());
        }

        //Initialise the robots with a view port so that the robot will be displayed on screen
        public void initialise(Viewport3D viewport)
        {
            kinectRobot = new Portable(viewport);
            fictionalRobot = new Portable();
            realAngles = new AnglePositions();
        }

        public void setRobotReady(RobotControllerLib.Robot robot)
        {
            this.robot = robot;
            robotReady = true;
            timer = new Timer(5); //get the timing going to keep syncrhonised.
            timer.AutoReset = true;
            timer.Elapsed += delegate { updateRealMotors(); };
            timer.Start(); //get going!
            //Alert all the listeners that the robot cannot be commanded.
            foreach (var listener in listeners)
            {
                listener.robotReady();
            }
        }

        //Return whether or not the robot is ready
        public bool isRobotReady()
        {
            return robotReady;
        }

        public void stopRobot()
        {
            //Stop the timer and alert the listeners
            robotReady = false;
            timer.Stop();
            timer.Dispose();
            foreach (var listener in listeners)
            {
                listener.robotReady();
            }
        }

        //This will return the real angles of the robot within the nearest 5 milliseconds approximately.
        public AnglePositions getRealAngles()
        {
            return realAngles;
        }

        //Updates the robot's angles throught the alternative thread
        private void updateRealMotors()
        {
            if (isRobotReady())
            {
                realAngles.LeftElbowAlong = robot.LeftArm.ElbowAlong.CurrentAngle;
                realAngles.LeftElbowOut = robot.LeftArm.ElbowOut.CurrentAngle;
                realAngles.LeftShoulderAlong = robot.LeftArm.ShoulderAlong.CurrentAngle;
                realAngles.LeftShoulderOut = robot.LeftArm.ShoulderOut.CurrentAngle;
                realAngles.RightElbowAlong = robot.RightArm.ElbowAlong.CurrentAngle;
                realAngles.RightElbowOut = robot.RightArm.ElbowOut.CurrentAngle;
                realAngles.RightShoulderAlong = robot.RightArm.ShoulderAlong.CurrentAngle;
                realAngles.RightShoulderOut = robot.RightArm.ShoulderOut.CurrentAngle;
            }
        }

        public Portable getKinectRobot()
        {
            return kinectRobot;
        }

        //Get the robot to play with. This will be null if the class has not been properly intialised!
        public Portable getPretendRobot()
        {
            //for playing with
            return fictionalRobot;
        }

        //Tell the class to try setting the real robot's angles.
        public void commitAngles(AnglePositions angles)
        {

            kinectRobot.setAngles(angles); //for display purposes
            if (isRobotReady())
            {
                robot.SetAngles(angles);
            }
        }

        //For sending in data from the Kinect
        public void kinectDataIn(AnglePositions angles)
        {
            
            foreach (var listener in listeners)
            {
                listener.kinectAngles(angles);
            }
        }

        /// <summary>
        /// This will enable the real robot to be displayed on screen
        /// </summary>
        /// <param name="keyHandler">The key handler to use.</param>
        public void setCameraToListenToKeys(I_Observable<I_KeyListener> keyHandler)
        {
            kinectRobot.setCameraToListenToKeys(keyHandler);
        }

        //Deal with the listeners:
        public void addListener(I_AngleListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(I_AngleListener listener)
        {
            listeners.Remove(listener);
        }
    }
}

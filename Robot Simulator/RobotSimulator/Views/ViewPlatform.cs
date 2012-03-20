using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Input;
using RobotSimulator.Model;
using System.Windows.Media;
using System.Windows.Controls;
using RobotSimulator.Utility;

namespace RobotSimulator.View
{
    /*
     * This class actually creates the camera, the lighting and puts the robot into the supplied view port.
     * It will also listen for keys and enable the user to view the robot from different angles!
     */ 
    class ViewPlatform : I_KeyListener,I_MotorListener
    {
        private PerspectiveCamera camera; //the camera used in the view port.
        private DirectionalLight light; //the light used in the view port.
        private FocussedCamera focussedCamera; //for helping to control the camera.
        private Robot robot; //the robot to display!
        private Model3DGroup outerModel;
        private MotorManager motorManager;
        public ViewPlatform(Viewport3D viewport, MotorManager motorManager)
        {
            this.motorManager = motorManager;
            //Add myself as a listener for the motor manager:
            this.motorManager.addListener(this);
            //Create a focussed camera to watch the origin:
            focussedCamera = new FocussedCamera(200, 0, 0);
            //Create the perspective camera!
            camera = getCamera(focussedCamera.Location, focussedCamera.Direction);
            viewport.Camera = camera;
            //Now to construct the light:
            light = getLight(Colors.White,focussedCamera.Direction);
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = light;
            viewport.Children.Add(visual);
            ModelVisual3D visual2 = new ModelVisual3D();
            //Now add in the robot too!
            robot = new Robot(viewport);
            updateMotors();
            outerModel = new Model3DGroup();
            outerModel.Children.Add(robot.getRobot());
            visual2.Content = outerModel;
            viewport.Children.Add(visual2);
            //Phew! That should be it...
            viewport.ClipToBounds = true;
        }

        //Sync the motor positions with the motor manager:
        private void updateMotors() {
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

        //Construct the default camera:
        private PerspectiveCamera getCamera(Point3D position, Vector3D lookDirection)
        {
            camera = new PerspectiveCamera();
            camera.FarPlaneDistance = 1000; //make it huge, since I don't want any weird effects
            camera.NearPlaneDistance = 1; //tiny for the same reasons.
            camera.UpDirection = new Vector3D(0, 1, 0); //let the view up be in the y direction by default:
            camera.Position = position;
            camera.LookDirection = lookDirection;
            camera.FieldOfView = 80; //give it a fairly wide field of view:
            return camera;
        }

        //Construct a light for the 3d view with the specified colour and direction
        private DirectionalLight getLight(Color colour, Vector3D direction)
        {
            light = new DirectionalLight();
            light.Color = colour;
            light.Direction = direction; //in the same direction as the camera
            return light;
        }

        public void KeyPressed(KeyEventArgs e)
        {
            //Warning: memory hog at the moment. Does not combine 3D rotations!
            switch (e.Key)
            {
                case Key.S:
                    //Rotate the object 
                    outerModel.Children[0] = Transforms.applyTransform((Model3DGroup)outerModel.Children[0], Transforms.makeAxisTransform(Axis.X, 5));
                    break;
                case Key.A:
                    //Reduce the azimuth:
                    outerModel.Children[0] = Transforms.applyTransform((Model3DGroup)outerModel.Children[0], Transforms.makeAxisTransform(Axis.Y, 5));
                    break;
                case Key.D:
                    //Increase the azimuth:
                    outerModel.Children[0] = Transforms.applyTransform((Model3DGroup)outerModel.Children[0], Transforms.makeAxisTransform(Axis.Y, -5));
                    break;
                case Key.W:
                    //Increase the inclination:
                    outerModel.Children[0] = Transforms.applyTransform((Model3DGroup)outerModel.Children[0], Transforms.makeAxisTransform(Axis.X, -5));
                    break;
                case Key.E:
                    //Increase the inclination:
                    outerModel.Children[0] = Transforms.applyTransform((Model3DGroup)outerModel.Children[0], Transforms.makeAxisTransform(Axis.Z, 5));
                    break;
                case Key.F:
                    //Increase the inclination:
                    outerModel.Children[0] = Transforms.applyTransform((Model3DGroup)outerModel.Children[0], Transforms.makeAxisTransform(Axis.Z, -5));
                    break;
                case Key.Z:
                    //Zoom in:
                    focussedCamera.Zoom+=5;
                    break;
                case Key.X:
                    //Zoom out:
                    focussedCamera.Zoom-=5;
                    break;
                case Key.C:
                    System.Windows.MessageBox.Show("Azimuth: " + focussedCamera.Azimuth.ToString() +
                        "\nInclination: " + focussedCamera.Inclination.ToString() +
                        "\nZoom: " + focussedCamera.Zoom.ToString() +
                        "\nLocation: " + focussedCamera.Location.ToString());
                    break;
                default:
                    break;
            }
            camera.Position = focussedCamera.Location;
            camera.LookDirection = focussedCamera.Direction;
            light.Direction = focussedCamera.Direction;
        }

        public void KeyReleased(KeyEventArgs e)
        {
            //Don't do anything yet...
        }

        public void motorSetTo(int motor, double degrees)
        {
            //Don't care
        }

        public void motorMovedTo(int motor, double degrees)
        {
            //Update the motor position on the robot.
            //The incoming value is in degrees from 0 to 255 (roughly), so adjust to
            //360 degrees first:
            degrees = ((degrees / 255) * 360);
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
    }
}

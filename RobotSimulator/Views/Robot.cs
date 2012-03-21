using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Threading;
using System.Timers;
using System.Windows.Controls;
using RobotSimulator.Utility;

namespace RobotSimulator.Model
{
    /*
     * The robot itself!
     * This has many parts...
     */
    class Robot
    {
        //The general cuboid parameters:
        class Cuboid
        {
            public double XLength;
            public double YLength;
            public double ZLength;
            //A quick way to set all of the lengths:
            public void setLengths(double xLength, double yLength, double zLength)
            {
                this.XLength = xLength;
                this.YLength = yLength;
                this.ZLength = zLength;
            }
            //Scale the entire size of the cuboid
            public void scale(double scale)
            {
                this.XLength = this.XLength * scale;
                this.YLength = this.YLength * scale;
                this.ZLength = this.ZLength * scale;
            }
        }
        //All of the different cuboids that make up the robot:
        private Cuboid Base = new Cuboid();
        private Cuboid Chest = new Cuboid();
        private Cuboid Neck = new Cuboid();
        private Cuboid Head = new Cuboid();
        private Cuboid RightArmJoin = new Cuboid();
        private Cuboid RightUpperArm = new Cuboid();
        private Cuboid RightLowerArm = new Cuboid();
        private Cuboid RightHand = new Cuboid();
        private Cuboid LeftArmJoin = new Cuboid();
        private Cuboid LeftUpperArm = new Cuboid();
        private Cuboid LeftLowerArm = new Cuboid();
        private Cuboid LeftHand = new Cuboid();
        //The transforms corresponding to the motors:
        private RotateTransform3D neckMotorTransform1 = Transforms.makeAxisTransform(Axis.Y, 0);
        private RotateTransform3D chestMotorTransform1 = Transforms.makeAxisTransform(Axis.Y, 0);
        private RotateTransform3D rightShoulderMotorTransform1 = Transforms.makeAxisTransform(Axis.X, 0);
        private RotateTransform3D rightShoulderMotorTransform2 = Transforms.makeAxisTransform(Axis.Z, 0);
        private RotateTransform3D rightElbowMotorTransform1 = Transforms.makeAxisTransform(Axis.X, 0);
        private RotateTransform3D rightElbowMotorTransform2 = Transforms.makeAxisTransform(Axis.Z, 0);
        private RotateTransform3D leftShoulderMotorTransform1 = Transforms.makeAxisTransform(Axis.X, 0);
        private RotateTransform3D leftShoulderMotorTransform2 = Transforms.makeAxisTransform(Axis.Z, 0);
        private RotateTransform3D leftElbowMotorTransform1 = Transforms.makeAxisTransform(Axis.X, 0);
        private RotateTransform3D leftElbowMotorTransform2 = Transforms.makeAxisTransform(Axis.Z, 0);
        //The colours for each part of the robot:
        private Color neckColour = Colors.Red;
        private Color headColour = Colors.Yellow;
        private Color chestColour = Colors.Green;
        private Color baseColour = Colors.Black;
        private Color rightArmJoinColour = Colors.Blue;
        private Color rightUpperArmColour = Colors.Pink;
        private Color rightLowerArmColour = Colors.Orange;
        private Color rightHandColour = Colors.Purple;
        private Color leftArmJoinColour = Colors.Brown;
        private Color leftUpperArmColour = Colors.YellowGreen;
        private Color leftLowerArmColour = Colors.Turquoise;
        private Color leftHandColour = Colors.DarkKhaki;
        //These are the default offsets of each motor from the standing position.
        //These should be set to align the robot's motors with the virtual motors set in here:
        private const double neckMotorOffset1 = 0;
        private const double chestMotorOffset1 = 0;
        private const double rightShoulderMotorOffset1 = 0;
        private const double rightShoulderMotorOffset2 = 0;
        private const double rightElbowMotorOffset1 = 0;
        private const double rightElbowMotorOffset2 = 0;
        private const double leftShoulderMotorOffset1 = 0;
        private const double leftShoulderMotorOffset2 = 0;
        private const double leftElbowMotorOffset1 = 0;
        private const double leftElbowMotorOffset2 = 0;
        //These can invert every motor to allow for any initial setup:
        private const bool neckMotorInvert1 = false;
        private const bool chestMotorInvert1 = false;
        private const bool rightShoulderMotorInvert1 = true;
        private const bool rightShoulderMotorInvert2 = true;
        private const bool rightElbowMotorInvert1 = true;
        private const bool rightElbowMotorInvert2 = true;
        private const bool leftShoulderMotorInvert1 = true;
        private const bool leftShoulderMotorInvert2 = false;
        private const bool leftElbowMotorInvert1 = true;
        private const bool leftElbowMotorInvert2 = false;
        //Some factor calculations:
        private static double boolToDouble(bool b)
        {
            if (b) { return -1; } else { return 1; }
        }
        private static double neckMotorFactor1 = boolToDouble(neckMotorInvert1);
        private static double chestMotorFactor1 = boolToDouble(chestMotorInvert1);
        private static double rightShoulderMotorFactor1 = boolToDouble(rightShoulderMotorInvert1);
        private static double rightShoulderMotorFactor2 = boolToDouble(rightShoulderMotorInvert2);
        private static double rightElbowMotorFactor1 = boolToDouble(rightElbowMotorInvert1);
        private static double rightElbowMotorFactor2 = boolToDouble(rightElbowMotorInvert2);
        private static double leftShoulderMotorFactor1 = boolToDouble(leftShoulderMotorInvert1);
        private static double leftShoulderMotorFactor2 = boolToDouble(leftShoulderMotorInvert2);
        private static double leftElbowMotorFactor1 = boolToDouble(leftElbowMotorInvert1);
        private static double leftElbowMotorFactor2 = boolToDouble(leftElbowMotorInvert2);
        //A load of annoying dispatched methods have to be included...
        //The motors that allow our robot to move!
        private double neckMotor1;
        public double NeckMotor1
        {
            get
            {
                return neckMotor1;
            }
            set
            {
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(neckMoveMotor1), value);
            }
        }
        private void neckMoveMotor1(double val)
        {
            neckMotorTransform1.Rotation = Transforms.makeAxisTransform(Axis.Y, neckMotorFactor1 * (val + neckMotorOffset1)).Rotation;
            neckMotor1 = val;
        }
        private double rightShoulderMotor1;
        public double RightShoulderMotor1
        {
            get
            {
                return rightShoulderMotor1;
            }
            set
            {
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(rightShoulderMoveMotor1), value);
            }
        }
        private void rightShoulderMoveMotor1(double val)
        {
            rightShoulderMotorTransform1.Rotation = Transforms.makeAxisTransform(Axis.X, rightShoulderMotorFactor1 * (val + rightShoulderMotorOffset1)).Rotation;
            rightShoulderMotor1 = val;
            //Update the other side too:
            rightShoulderMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(rightShoulderMotorFactor1 * (rightShoulderMotor1 + rightShoulderMotorOffset1))), Math.Cos(MoreMaths.DegToRad(rightShoulderMotorFactor1 * (rightShoulderMotor1 + rightShoulderMotorOffset1)))), rightShoulderMotorFactor2 * (rightShoulderMotor2 + rightShoulderMotorOffset2)).Rotation;
        }
        private double rightShoulderMotor2;
        public double RightShoulderMotor2
        {
            get
            {
                return rightShoulderMotor2;
            }
            set
            {
                //The axis in which this rotates depends upon how far the first motor has rotated about
                //its x axis. In other words, rotate the z axis about the x axis
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(rightShoulderMoveMotor2), value);
            }
        }
        private void rightShoulderMoveMotor2(double val)
        {
            rightShoulderMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(rightShoulderMotorFactor1 * (RightShoulderMotor1 + rightShoulderMotorOffset1))), Math.Cos(MoreMaths.DegToRad(rightShoulderMotorFactor1 * (RightShoulderMotor1 + rightShoulderMotorOffset1)))), rightShoulderMotorFactor2 * (val + rightShoulderMotorOffset2)).Rotation;
            rightShoulderMotor2 = val;
        }
        private double rightElbowMotor1;
        public double RightElbowMotor1
        {
            get
            {
                return rightElbowMotor1;
            }
            set
            {
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(rightElbowMoveMotor1), value);
            }
        }
        private void rightElbowMoveMotor1(double val)
        {
            rightElbowMotorTransform1.Rotation = Transforms.makeAxisTransform(Axis.X, rightElbowMotorFactor1 * (val + rightElbowMotorOffset1)).Rotation;
            rightElbowMotor1 = val;
            //Update the other side too:
            rightElbowMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(rightElbowMotorFactor1 * (rightElbowMotor1 + rightElbowMotorOffset1))), Math.Cos(MoreMaths.DegToRad(rightElbowMotorFactor1 * (rightElbowMotor1 + rightElbowMotorOffset1)))), rightElbowMotorFactor2 * (rightElbowMotor2 + rightElbowMotorOffset2)).Rotation;
        }
        private double rightElbowMotor2;
        public double RightElbowMotor2
        {
            get
            {
                return rightElbowMotor2;
            }
            set
            {
                //The axis in which this rotates depends upon how far the first motor has rotated about
                //its x axis. In other words, rotate the z axis about the x axis
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(rightElbowMoveMotor2), value);
            }
        }
        private void rightElbowMoveMotor2(double val)
        {
            rightElbowMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(rightElbowMotorFactor1 * (RightElbowMotor1 + rightElbowMotorOffset1))), Math.Cos(MoreMaths.DegToRad(rightElbowMotorFactor1 * (RightElbowMotor1 + rightElbowMotorOffset1)))), rightElbowMotorFactor2 * (val + rightElbowMotorOffset2)).Rotation;
            rightElbowMotor2 = val;
        }
        private double leftShoulderMotor1;
        public double LeftShoulderMotor1
        {
            get
            {
                return leftShoulderMotor1;
            }
            set
            {
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(leftShoulderMoveMotor1), value);
            }
        }
        private void leftShoulderMoveMotor1(double val)
        {
            leftShoulderMotorTransform1.Rotation = Transforms.makeAxisTransform(Axis.X, leftShoulderMotorFactor1 * (val + leftShoulderMotorOffset1)).Rotation;
            leftShoulderMotor1 = val;
            //Update the other side too:
            leftShoulderMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(leftShoulderMotorFactor1 * (leftShoulderMotor1 + leftShoulderMotorOffset1))), Math.Cos(MoreMaths.DegToRad(leftShoulderMotorFactor1 * (leftShoulderMotor1 + leftShoulderMotorOffset1)))), leftShoulderMotorFactor2 * (leftShoulderMotor2 + leftShoulderMotorOffset2)).Rotation;
        }
        private double leftShoulderMotor2;
        public double LeftShoulderMotor2
        {
            get
            {
                return leftShoulderMotor2;
            }
            set
            {
                //The axis in which this rotates depends upon how far the first motor has rotated about
                //its x axis. In other words, rotate the z axis about the x axis
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(leftShoulderMoveMotor2), value);
            }
        }
        private void leftShoulderMoveMotor2(double val)
        {
            leftShoulderMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(leftShoulderMotorFactor1 * (LeftShoulderMotor1 + leftShoulderMotorOffset1))), Math.Cos(MoreMaths.DegToRad(leftShoulderMotorFactor1 * (LeftShoulderMotor1 + leftShoulderMotorOffset1)))), leftShoulderMotorFactor2 * (val + leftShoulderMotorOffset2)).Rotation;
            leftShoulderMotor2 = val;
        }
        private double leftElbowMotor1;
        public double LeftElbowMotor1
        {
            get
            {
                return leftElbowMotor1;
            }
            set
            {
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(leftElbowMoveMotor1), value);
            }
        }
        private void leftElbowMoveMotor1(double val)
        {
            leftElbowMotorTransform1.Rotation = Transforms.makeAxisTransform(Axis.X, leftElbowMotorFactor1 * (val + leftElbowMotorOffset1)).Rotation;
            leftElbowMotor1 = val;
            //Update the other side too:
            leftElbowMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(leftElbowMotorFactor1 * (leftElbowMotor1 + leftElbowMotorOffset1))), Math.Cos(MoreMaths.DegToRad(leftElbowMotorFactor1 * (leftElbowMotor1 + leftElbowMotorOffset1)))), leftElbowMotorFactor2 * (leftElbowMotor2 + leftElbowMotorOffset2)).Rotation;
        }
        private double leftElbowMotor2;
        public double LeftElbowMotor2
        {
            get
            {
                return leftElbowMotor2;
            }
            set
            {
                //The axis in which this rotates depends upon how far the first motor has rotated about
                //its x axis. In other words, rotate the z axis about the x axis
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(leftElbowMoveMotor2), value);
            }
        }
        private void leftElbowMoveMotor2(double val)
        {
            leftElbowMotorTransform2.Rotation = Transforms.makeAxisTransform(new Vector3D(0, -Math.Sin(MoreMaths.DegToRad(leftElbowMotorFactor1 * (LeftElbowMotor1 + leftElbowMotorOffset1))), Math.Cos(MoreMaths.DegToRad(leftElbowMotorFactor1 * (LeftElbowMotor1 + leftElbowMotorOffset1)))), leftElbowMotorFactor2 * (val + leftElbowMotorOffset2)).Rotation;
            leftElbowMotor2 = val;
        }
        private double chestMotor1;
        public double ChestMotor1
        {
            get
            {
                return chestMotor1;
            }
            set
            {
                viewport.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(chestMoveMotor1), value);
            }
        }
        private void chestMoveMotor1(double val)
        {
            chestMotorTransform1.Rotation = Transforms.makeAxisTransform(Axis.Y, chestMotorFactor1 * (val + chestMotorOffset1)).Rotation;
            chestMotor1 = val;
        }

        //This factor can scale up the entire robot!
        private double scale;
        //The robot itself!!
        private Model3DGroup robot;
        private Viewport3D viewport;
        //For rather complicated reasons, the robot needs the viewport
        //in order to control the threads that affect it. Otherwise, chaos ensues...
        public Robot(Viewport3D viewport)
        {
            this.viewport = viewport;
            //The right arm:
            this.RightHand.setLengths(10, 10, 10);
            this.RightLowerArm.setLengths(10, 40, 10);
            this.RightUpperArm.setLengths(10, 40, 10);
            this.RightArmJoin.setLengths(10, 10, 10);
            //The left arm:
            this.LeftHand.setLengths(10, 10, 10);
            this.LeftLowerArm.setLengths(10, 40, 10);
            this.LeftUpperArm.setLengths(10, 40, 10);
            this.LeftArmJoin.setLengths(10, 10, 10);
            //The rest of the body:
            this.Neck.setLengths(10, 10, 10);
            this.Head.setLengths(20, 20, 10);
            this.Chest.setLengths(40, 120, 10);
            this.Base.setLengths(100, 20, 100);
            //Set all the motors to cause the robot to stand:
            //Default:
            this.NeckMotor1 = -neckMotorOffset1;
            this.ChestMotor1 = -chestMotorOffset1;
            this.LeftElbowMotor1 = -leftElbowMotorOffset1;
            this.LeftElbowMotor2 = -leftElbowMotorOffset2;
            this.LeftShoulderMotor1 = -leftShoulderMotorOffset1;
            this.LeftShoulderMotor2 = -leftShoulderMotorOffset2;
            this.RightElbowMotor1 = -rightElbowMotorOffset1;
            this.RightElbowMotor2 = -rightElbowMotorOffset2;
            this.RightShoulderMotor1 = -rightShoulderMotorOffset1;
            this.RightShoulderMotor2 = -rightShoulderMotorOffset2;
            //Scale everything by the specified amount:
            scale = 1;
            this.Head.scale(scale);
            this.LeftArmJoin.scale(scale);
            this.LeftHand.scale(scale);
            this.LeftLowerArm.scale(scale);
            this.LeftUpperArm.scale(scale);
            this.Neck.scale(scale);
            this.Base.scale(scale);
            this.Chest.scale(scale);
            this.RightArmJoin.scale(scale);
            this.RightHand.scale(scale);
            this.RightLowerArm.scale(scale);
            this.RightUpperArm.scale(scale);
            //Now construct the basic robot:
            construct();
        }

        public Model3DGroup getRobot()
        {
            return robot;
        }

        //Construct the robot from the specific values, setting the rotation transforms with the motors:
        private void construct()
        {
            //Construct the robot according to the graph!
            robot = new Model3DGroup();
            //Firstly, construct all of the pieces in the central position on the grid (transformations occur later):
            Model3DGroup headObj = Shapes.makeCuboid(Head.XLength, Head.YLength, Head.ZLength, headColour);
            Model3DGroup neckObj = Shapes.makeCuboid(Neck.XLength, Neck.YLength, Neck.ZLength, neckColour);
            Model3DGroup chestObj = Shapes.makeCuboid(Chest.XLength, Chest.YLength, Chest.ZLength, chestColour);
            Model3DGroup baseObj = Shapes.makeCuboid(Base.XLength, Base.YLength, Base.ZLength, baseColour);
            Model3DGroup leftArmJoinObj = Shapes.makeCuboid(LeftArmJoin.XLength, LeftArmJoin.YLength, LeftArmJoin.ZLength, leftArmJoinColour);
            Model3DGroup leftUpperArmObj = Shapes.makeCuboid(LeftUpperArm.XLength, LeftUpperArm.YLength, LeftUpperArm.ZLength, leftUpperArmColour);
            Model3DGroup leftLowerArmObj = Shapes.makeCuboid(LeftLowerArm.XLength, LeftLowerArm.YLength, LeftLowerArm.ZLength, leftLowerArmColour);
            Model3DGroup leftHandObj = Shapes.makeCuboid(LeftHand.XLength, LeftHand.YLength, LeftHand.ZLength, leftHandColour);
            Model3DGroup rightArmJoinObj = Shapes.makeCuboid(RightArmJoin.XLength, RightArmJoin.YLength, RightArmJoin.ZLength, rightArmJoinColour);
            Model3DGroup rightUpperArmObj = Shapes.makeCuboid(RightUpperArm.XLength, RightUpperArm.YLength, RightUpperArm.ZLength, rightUpperArmColour);
            Model3DGroup rightLowerArmObj = Shapes.makeCuboid(RightLowerArm.XLength, RightLowerArm.YLength, RightLowerArm.ZLength, rightLowerArmColour);
            Model3DGroup rightHandObj = Shapes.makeCuboid(RightHand.XLength, RightHand.YLength, RightHand.ZLength, rightHandColour);
            //Now just follow the graph very carefully:
            //(From the leaves up)

            //*************
            //The left arm:
            //*************

            //Firstly, translate the hand to move ready to be below the arm:
            leftHandObj = Transforms.applyTransform(leftHandObj, Transforms.makeTranslateTransform(0, -((LeftLowerArm.YLength / 2) + (LeftHand.YLength / 2)), 0));
            //Add the left hand to the lower left arm:
            Shapes.addChild(leftLowerArmObj, leftHandObj);
            //Translate to ready for motor:
            leftLowerArmObj = Transforms.applyTransform(leftLowerArmObj, Transforms.makeTranslateTransform(0, -(LeftLowerArm.YLength / 2), 0));
            //Attach elbow motor1:
            leftLowerArmObj = Transforms.applyTransform(leftLowerArmObj, leftElbowMotorTransform1); //Not working at the moment!
            //Attach second elbow motor2:
            leftLowerArmObj = Transforms.applyTransform(leftLowerArmObj, leftElbowMotorTransform2);

            //Again, translate the lower arm with the hand to be ready to fit onto the upper arm:
            leftLowerArmObj = Transforms.applyTransform(leftLowerArmObj, Transforms.makeTranslateTransform(0, -(LeftUpperArm.YLength / 2), 0));
            //Add the left lower arm to the left upper arm:
            Shapes.addChild(leftUpperArmObj, leftLowerArmObj);
            //Add the motors:
            //Translate to ready for motor:
            leftUpperArmObj = Transforms.applyTransform(leftUpperArmObj, Transforms.makeTranslateTransform(0, (-(LeftUpperArm.YLength / 2)) + (LeftArmJoin.YLength / 2), 0));
            //Attach shoulder motor1:
            leftUpperArmObj = Transforms.applyTransform(leftUpperArmObj, leftShoulderMotorTransform1); //Not working at the moment!
            //Attach second shoulder motor2:
            leftUpperArmObj = Transforms.applyTransform(leftUpperArmObj, leftShoulderMotorTransform2);
            //Translate to the side, so that the left join can be applied:
            leftUpperArmObj = Transforms.applyTransform(leftUpperArmObj, Transforms.makeTranslateTransform(
                ((LeftUpperArm.XLength / 2) + (LeftArmJoin.XLength / 2)),
                0,
                0));
            //Complete the arm!!
            Shapes.addChild(leftArmJoinObj, leftUpperArmObj);

            //**************
            //The right arm:
            //**************

            //Pretty much the same as the left arm with a few signs reversed:
            rightHandObj = Transforms.applyTransform(rightHandObj, Transforms.makeTranslateTransform(0, -((RightLowerArm.YLength / 2) + (RightHand.YLength / 2)), 0));
            Shapes.addChild(rightLowerArmObj, rightHandObj);
            //Translate to ready for motor:
            rightLowerArmObj = Transforms.applyTransform(rightLowerArmObj, Transforms.makeTranslateTransform(0, -(RightLowerArm.YLength / 2), 0));
            //Attach elbow motor1:
            rightLowerArmObj = Transforms.applyTransform(rightLowerArmObj, rightElbowMotorTransform1); //Not working at the moment!
            //Attach second elbow motor2:
            rightLowerArmObj = Transforms.applyTransform(rightLowerArmObj, rightElbowMotorTransform2);
            //Again, translate the lower arm with the hand to be ready to fit onto the upper arm:
            rightLowerArmObj = Transforms.applyTransform(rightLowerArmObj, Transforms.makeTranslateTransform(0, -(RightUpperArm.YLength / 2), 0));
            Shapes.addChild(rightUpperArmObj, rightLowerArmObj);
            //Add the motors:
            //Translate to ready for motor:
            rightUpperArmObj = Transforms.applyTransform(rightUpperArmObj, Transforms.makeTranslateTransform(0, (-(RightUpperArm.YLength / 2)) + (RightArmJoin.YLength / 2), 0));
            //Attach elbow motor1:
            rightUpperArmObj = Transforms.applyTransform(rightUpperArmObj, rightShoulderMotorTransform1); //Not working at the moment!
            //Attach second elbow motor2:
            rightUpperArmObj = Transforms.applyTransform(rightUpperArmObj, rightShoulderMotorTransform2);
            //Translate to the side, so that the right join can be applied:
            rightUpperArmObj = Transforms.applyTransform(rightUpperArmObj, Transforms.makeTranslateTransform(
                -((RightUpperArm.XLength / 2) + (RightArmJoin.XLength / 2)),
                0,
                0));
            Shapes.addChild(rightArmJoinObj, rightUpperArmObj);

            //*********
            //The neck:
            //*********

            //Translate the head to get it our of the way:
            headObj = Transforms.applyTransform(headObj, Transforms.makeTranslateTransform(0, (Head.YLength / 2) + (Neck.YLength / 2), 0));
            //Now stick the neck on!
            Shapes.addChild(neckObj, headObj);
            //Now add the motor to the neck!
            neckObj = Transforms.applyTransform(neckObj, neckMotorTransform1);

            //**********
            //The chest:
            //**********

            //Translate everybody into the right position ready to be accepted by the chest:
            neckObj = Transforms.applyTransform(neckObj, Transforms.makeTranslateTransform(0, (Chest.YLength / 2) + (Neck.YLength / 2), 0));
            leftArmJoinObj = Transforms.applyTransform(leftArmJoinObj, Transforms.makeTranslateTransform(
                ((LeftArmJoin.XLength / 2) + (Chest.XLength / 2)),
                ((LeftArmJoin.YLength / 2) + (Chest.YLength / 2)) - (LeftArmJoin.YLength),
                0));
            rightArmJoinObj = Transforms.applyTransform(rightArmJoinObj, Transforms.makeTranslateTransform(
                -((RightArmJoin.XLength / 2) + (Chest.XLength / 2)),
                ((RightArmJoin.YLength / 2) + (Chest.YLength / 2)) - (RightArmJoin.YLength),
                0));
            //Combine the arms, and neck as children of the chest
            Shapes.addChild(chestObj, Shapes.combine(neckObj, Shapes.combine(rightArmJoinObj, leftArmJoinObj)));
            //Now add the motor for the chest:
            chestObj = Transforms.applyTransform(chestObj, chestMotorTransform1);


            //*********
            //The base:
            //*********

            //Translate the chest ready to accept the base:
            chestObj = Transforms.applyTransform(chestObj, Transforms.makeTranslateTransform(0, (Chest.YLength / 2) + (Base.YLength / 2), 0));
            Shapes.addChild(baseObj, chestObj);

            //Finish the robot itself!
            //Correct the vertical height, so that the centre of the robot really is at (0,0,0)
            baseObj = Transforms.applyTransform(baseObj, Transforms.makeTranslateTransform(0,
                (Base.YLength / 2) - ((Base.YLength + Chest.YLength + Neck.YLength + Head.YLength) / 2),
                0));
            //The finished robot!
            Shapes.addChild(robot, baseObj);
        }
    }
}

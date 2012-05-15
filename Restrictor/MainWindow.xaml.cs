using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RobotSimulator.View;
using RobotSimulator.Utility;
using RobotSimulator.Model;
using RobotSimulator.Controller;
using Microsoft.Kinect;
using Restrictor;
using System.Threading;


namespace RobotSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,I_Observable<I_KeyListener>
    {
        private ICollection<I_KeyListener> key_listeners = new LinkedList<I_KeyListener>();

        private CollisionRestrictor synchroniser = CollisionRestrictor.INSTANCE;
        //*****************************************CHANGED**********************************
        //Linker linker;

        bool closing = false;

        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        AngleCalculator angleCalculator;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Initialise the display:
            synchroniser.initialise(viewport);
            synchroniser.setCameraToListenToKeys(this); //for the camera
            //Set everybody up...
            KeyInputController controller = new KeyInputController(synchroniser.getKinectRobot().motors(),
                synchroniser.getKinectRobot().positions());
            this.addListener(controller); //allow him to listen to keys
            //Set up the collision detector:
            AngleAdapter angleAdapter = new AngleAdapter(synchroniser.getPretendRobot());
            synchroniser.addListener(angleAdapter);

            //linker = new Linker(this,viewport);

            angleCalculator = new AngleCalculator(null);
            //An example of how you might use the Portable class
            //Portable p = new Portable(viewport);
            //p.setCameraToListenToKeys(this);
            //p.setMotor(MotorManager.LEFT_SHOULDER_MOTOR1, 90);

            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);

            initialiseRobot();
        }

        public void initialiseRobot()
        {
            //Set up the real robot: DISABLED - NULL POINTER EXCEPTION WITHOUT ROBOT CONNECTED
            RobotControllerLib.Robot robot = new RobotControllerLib.Robot();
            synchroniser.setRobotReady(robot);
        }

        //KEY EVENTS ***********************************
        private void viewport_KeyDown(object sender, KeyEventArgs e)
        {
            //Forward the key down event to all the listeners:
            foreach (var listener in key_listeners)
            {
                listener.KeyPressed(e);
            }
        }
        private void viewport_KeyUp(object sender, KeyEventArgs e)
        {
            //Forward the key up event to all the listeners:
            foreach (var listener in key_listeners)
            {
                listener.KeyReleased(e);
            }
        }

        public void addListener(I_KeyListener listener)
        {
            key_listeners.Add(listener);
        }

        public void removeListener(I_KeyListener listener)
        {
            key_listeners.Remove(listener);
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            // set the new sensor to be the active one
            if (e != null)
            {
                KinectSensor oldSensor = (KinectSensor)e.OldValue;
                if (oldSensor != null)
                {
                    StopKinect(oldSensor);
                }

                KinectSensor newSensor = (KinectSensor)e.NewValue;

                if (newSensor != null)
                {
                    newSensor.ColorStream.Enable();
                    newSensor.DepthStream.Enable();

                    // a few smoothing parameters
                    var parameters = new TransformSmoothParameters
                    {
                        Smoothing = 0.5f,
                        Correction = 0.5f,
                        Prediction = 0.8f,
                        JitterRadius = 0.3f,
                        MaxDeviationRadius = 1.0f



                    };
                    newSensor.SkeletonStream.Enable(parameters);

                    // attach the appropriate event to call each frame
                    newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);

                    // enable the depthstream and colorstream on the kinect sensor
                    newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    try
                    {
                        newSensor.Start();
                    }
                    catch (System.IO.IOException)
                    {
                        // called when another device is trying to communicate with the sensor
                        kinectSensorChooser1.AppConflictOccurred();
                    }
                }
            }
        }

        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

            if (closing)
                return;
            {
            }
            // set the active skeleton to be the first deteceted
            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
            {

                return;
            }

            // calls the method which does all the calculations
            GetCameraPoint(first, e);
        }
        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                // puts all the skeletons in a array called "allSkeletons"
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                // selects the first tracked skeleton in the array
                Skeleton first = (from s in allSkeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();

                return first;
            }
        }

        private int counter = 0;
        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            if (e == null) return;
            counter++;
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null || kinectSensorChooser1.Kinect == null)
                {
                    return;
                }


                // a counter introduced to slow the frame rate effectively         
                if (counter >= 1)
                {
                    //load the new skeleton into the angleCalculator
                    angleCalculator.updateSkeleton(first);

                    //read the angles from the angleCalculator

                    AngleSet angles = angleCalculator.getAngles();
                    //********************************CHANGED********************************
                    // tell the robotSimulator the new angles
                    //linker.updateMotors(angles);
                    synchroniser.kinectDataIn(angles.toAnglePositions());
                    
                    counter = 0;
                }

                // moves each of the ellipsis over the appropriate joint
                followElementOver(first, depth, lefthandellipse, JointType.HandLeft);
                followElementOver(first, depth, righthandellipse, JointType.HandRight);
                followElementOver(first, depth, leftelbowellipse, JointType.ElbowLeft);
                followElementOver(first, depth, rightelbowellipse, JointType.ElbowRight);
                followElementOver(first, depth, leftshoulderellipse, JointType.ShoulderLeft);
                followElementOver(first, depth, rightshoulderellipse, JointType.ShoulderRight);
                followElementOver(first, depth, leftwristellipse, JointType.WristLeft);
                followElementOver(first, depth, rightwristellipse, JointType.WristRight);
                followElementOver(first, depth, headellipse, JointType.Head);
                followElementOver(first, depth, spineellipse, JointType.Spine);
                followElementOver(first, depth, lefthipellipse, JointType.HipLeft);
                followElementOver(first, depth, righthipellipse, JointType.HipRight);


            }
        }
        void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            // just positions the element over the point where it should be
            element.Margin = new Thickness(kinectColorViewer1.Margin.Left + point.X - element.Width / 2, kinectColorViewer1.Margin.Top + point.Y - element.Height / 2, 0, 0);

        }

        void followElementOver(Skeleton skeleton, DepthImageFrame depth, FrameworkElement element, JointType jointType)
        {

            // calculates the exact point using the colorPoint and the depthPoint separately
            DepthImagePoint depthPoint =
                depth.MapFromSkeletonPoint(skeleton.Joints[jointType].Position);

            ColorImagePoint colorPoint =
                    depth.MapToColorImagePoint(depthPoint.X, depthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);

            // then moves the element over that point
            CameraPosition(element, colorPoint);

        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                if (sensor.AudioSource != null)
                {
                    sensor.AudioSource.Stop();
                }
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
            if (kinectSensorChooser1 != null)
            {
                StopKinect(kinectSensorChooser1.Kinect);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using RobotSimulator.Utility;

namespace RobotSimulator.Model
{
    /*
     * This class implements a focussed camera. By this, I mean a camera that always looks towards the same point.
     * It can be rotated by the azimuth and inclination angles about this point, and can zoom in or out. However, it cannot
     * look in any other direction. This is handy for viewing an object in the simulator.
     * 
     * Note: does not support camera twist or clipping or anything aong those lines. This is really just a mathematical class.
     */
    class FocussedCamera
    {
        //Specify the point that the camera should look at:
        public Point3D LookAt;
        //The zoom - how far away the camera is from the point it is looking at:
        private double zoom;
        public double Zoom {
            get
            {
                return zoom;
            }
            set {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("FocussedCamera: Cannot zoom this far. The zoom must always be positive - greater than zero.", "zoom");
                }
                else
                {
                    zoom = value;
                }
            }
        }
        //The inclination - how far "down" from the z axis it is looking (0 means it stands on positive z)
        //IN DEGREES
        public double Inclination;
        //The Azimth - how far "right" from the x axis it is looking (anti-clockwsise about z axis)
        //IN DEGREES
        public double Azimuth;

        //Create a new focussed camera with the specified zoom, inclination and azimuth
        public FocussedCamera(double zoom, double inclination, double azimuth)
        {
            this.Azimuth = azimuth;
            this.Inclination = inclination;
            if (zoom <= 0)
            {
                throw new ArgumentOutOfRangeException("FocussedCamera: Cannot zoom this far. The zoom must always be positive - greater than zero.", "zoom");
            }
            this.Zoom = zoom;
            this.LookAt = new Point3D(0, 0, 0);
        }

        //Create a new focussed camera with the specified zoom, inclination, azimuth and point to look at:
        public FocussedCamera(double zoom, double inclination, double azimuth, Point3D lookAt)
        {
            this.Azimuth = azimuth;
            this.Inclination = inclination;
            if (zoom <= 0)
            {
                throw new ArgumentOutOfRangeException("FocussedCamera: Cannot zoom this far. The zoom must always be positive - greater than zero.", "zoom");
            }
            this.Zoom = zoom;
            this.LookAt = lookAt;
        }

        //Return the x coordinate of the camera:
        private double X { 
            get
            {
                return (Zoom * Math.Sin(MoreMaths.DegToRad(Inclination)) * Math.Cos(MoreMaths.DegToRad(Azimuth))) + LookAt.X;
            }
        }
        //Return the y coordinate of the camera:
        private double Y
        {
            get
            {
                return (Zoom * Math.Sin(MoreMaths.DegToRad(Inclination)) * Math.Sin(MoreMaths.DegToRad(Azimuth))) + LookAt.Y;
            }
        }
        //Return the z coordinate of the camera:
        private double Z
        {
            get
            {
                return (Zoom * Math.Cos(MoreMaths.DegToRad(Inclination))) + LookAt.Z;
            }
        }
        //Return the 3D point referring to the camera's location:
        public Point3D Location
        {
            get
            {
                return new Point3D(X, Y, Z);
            }
        }
        //Return the direction the camera is looking in:
        public Vector3D Direction
        {
            get
            {
                return new Vector3D(-X, -Y, -Z);
            }
        }
    }
}

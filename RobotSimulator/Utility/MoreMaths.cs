using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace RobotSimulator.Utility
{
    public static class MoreMaths
    {
        //For converting angles:
        public static double DegToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadToDeg(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        //Returns the angle to set a motor to - 0 to 255 -, to replicate the given degrees - 0 to 360 degrees -
        public static double toMotorAngle(double angle)
        {
            return ((angle * 255) / 360);
        }

        //Returns the original angle - 0 to 360 degrees - represented by the motor angle - 0 to 255 -
        public static double fromMotorAngle(double angle)
        {
            return ((angle * 360) / 255);
        }
        public static Point3D add(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }
        public static Point3D sub(Point3D p1, Point3D p2)
        {
            return add(p1, neg(p2));
        }
        public static Point3D neg(Point3D p)
        {
            return mul(p, -1);
        }
        public static Point3D mul(Point3D p, double s)
        {
            return new Point3D(p.X * s, p.Y * s, p.Z * s);
        }
        public static double dot(Point3D p1, Point3D p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }
        public static double lenSquared(Point3D p)
        {
            return dot(p, p);
        }
        public static double length(Point3D p)
        {
            return Math.Sqrt(lenSquared(p));
        }
        public static double dSquared(Point3D p1, Point3D p2)
        {
            return lenSquared(sub(p1, p2));
        }
        public static double distance(Point3D p1, Point3D p2)
        {
            return Math.Sqrt(dSquared(p1, p2));
        }
        public static Point3D cross(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.Y * p2.Z - p1.Z * p2.Y, p1.Z * p2.X - p1.X * p2.Z, p1.X * p2.Y - p1.Y * p2.X);
        }
        public static Point3D norm(Point3D p)
        {
            return mul(p, 1 / length(p));
        }
        public static Point3D project(Point3D p, Point3D l1, Point3D l2)
        {
            Point3D v = sub(l2, l1);
            return add(l1, mul(v, dot(v, sub(p, l1)) / lenSquared(v)));
        }
        public static Point3D project(Point3D p, Point3D p1, Point3D p2, Point3D p3)
        {
            return project(p1, p, add(p, cross(sub(p1, p2), sub(p3, p1))));
        }
        public static Point3D toPoint(this Vector3D v)
        {
            return new Point3D(v.X, v.Y, v.Z);
        }

    }
}

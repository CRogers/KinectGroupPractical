using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using RobotSimulator.Model;
using RobotSimulator.View;
using MM = RobotSimulator.Utility.MoreMaths;

namespace RobotSimulator
{
    public class CollisionDetector
    {
        public static double[] getDistances(Point3D[] keyPoints,
            double torsoRadius, double armRadius, double forearmRadius)//negative means intersection, positive is fine
        {
            Point3D le = keyPoints[0],
                ls = keyPoints[1],
                lh = keyPoints[2],
                re = keyPoints[3],
                rs = keyPoints[4],
                rh = keyPoints[5],
                b = keyPoints[6],
                h = keyPoints[7];
            return new double[]{
                distance(le,lh,b,h)-forearmRadius-torsoRadius,
                distance(le,lh,rs,re)-forearmRadius-armRadius,
                distance(le,lh,re,rh)-forearmRadius*2,
                distance(ls,le,re,rh)-armRadius-forearmRadius,
                distance(b,h,re,rh)-torsoRadius-forearmRadius
            };
        }

        public static double distance(Point3D p1, Point3D p2, Point3D op1, Point3D op2)
        {
            Point3D vcross = MM.cross(MM.norm(MM.sub(op2, op1)), MM.norm(MM.sub(p1, p2)));
            if (MM.lenSquared(vcross) < .001)
            {
                double[] vdot = {
                                    MM.dot(MM.sub(p1,op1),MM.sub(op2,op1))/MM.dSquared(op2,op1),
                                    MM.dot(MM.sub(p2,op1),MM.sub(op2,op1))/MM.dSquared(op2,op1),
                                    MM.dot(MM.sub(op1,p1),MM.sub(p2,p1))/MM.dSquared(p2,p1),
                                    MM.dot(MM.sub(op2,p1),MM.sub(p2,p1))/MM.dSquared(p2,p1)
                                };
                for (int x = 0; x < 4; x++)
                    if (vdot[x] >= 0 && vdot[x] <= 1)
                        return MM.distance(MM.project(op1, p1, p2), op1);
                return Math.Min(Math.Min(MM.distance(p1, op1), MM.distance(p2, op1)),
                    Math.Min(MM.distance(p1, op2), MM.distance(p2, op2)));
            }
            Point3D p3 = MM.add(p1, MM.sub(op2, op1));
            Point3D a = MM.project(op1, p1, p2, p3), b = MM.project(op2, p1, p2, p3);
            if (MM.dot(MM.cross(MM.sub(a, p1), MM.sub(p2, p1)), MM.cross(MM.sub(b, p1), MM.sub(p2, p1))) <= 0
                && MM.dot(MM.cross(MM.sub(p1, a), MM.sub(b, a)), MM.cross(MM.sub(p2, a), MM.sub(b, a))) <= 0)
                return MM.distance(a, op1);
            double dist = Double.MaxValue;
            foreach (Point3D p in new Point3D[] { p1, p2 })
            {
                Point3D proj = MM.project(p, op1, op2);
                if (MM.dot(MM.sub(proj, op1), MM.sub(op2, proj)) >= 0)
                    dist = Math.Min(dist, MM.distance(proj, p));
            }
            foreach (Point3D p in new Point3D[] { op1, op2 })
            {
                Point3D proj = MM.project(p, p1, p2);
                if (MM.dot(MM.sub(proj, p1), MM.sub(p2, proj)) >= 0)
                    dist = Math.Min(dist, MM.distance(proj, p));
            }
            return Math.Min(dist, Math.Min(Math.Min(MM.distance(p1, op1), MM.distance(p2, op1)),
                Math.Min(MM.distance(p1, op2), MM.distance(p2, op2))));
        }
    }
}
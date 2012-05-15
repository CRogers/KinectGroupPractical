using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;
using RobotSimulator.Utility;
using MM = RobotSimulator.Utility.MoreMaths;
using T = RobotSimulator.Model.Transforms;
using RobotSimulator.Model;

namespace RobotSimulator
{
    public class AngleCalculator
    {
        Vector3D rightWrist;
        Vector3D rightElbow;
        Vector3D rightShoulder;
        Vector3D rightHip;
        Vector3D head;
        Vector3D spine;
        Vector3D leftWrist;
        Vector3D leftElbow;
        Vector3D leftShoulder;
        Vector3D leftHip;
        Skeleton skeleton;
        AngleSet angles;

        public AngleCalculator(Skeleton skeleton)
        {
            // initialises the skeleton and the angles
             updateSkeleton(skeleton);
             angles = new AngleSet();
        }

        private double computeAngle(Point3D p1, Point3D p2, Point3D axis)
        {

            double angle = Math.Acos(MM.dot(MM.norm(p1), MM.norm(p2)));
            if(MM.dot(MM.cross(p1,p2),axis)<0)
                angle*=-1;
            return angle;
        }

        public void updateSkeleton(Skeleton newSkeleton)
        {
            // this code just updates all the vectors and update all the angles
            if (newSkeleton != null)
            {
                this.skeleton = newSkeleton;
                rightWrist = getVector(skeleton, JointType.WristRight);

                rightElbow = getVector(skeleton, JointType.ElbowRight);

                rightShoulder = getVector(skeleton, JointType.ShoulderRight);

                rightHip = getVector(skeleton, JointType.HipRight);

                head = getVector(skeleton, JointType.Head);

                spine = getVector(skeleton, JointType.Spine);

                leftWrist = getVector(skeleton, JointType.WristLeft);

                leftElbow = getVector(skeleton, JointType.ElbowLeft);

                leftShoulder = getVector(skeleton, JointType.ShoulderLeft);

                leftHip = getVector(skeleton, JointType.HipLeft);
                updateAngles();
            }
        }

        public AngleSet getAngles()
        {
            // return all the angles
            return angles;
        }

        private void updateAngles()
        {
            Point3D ls = MM.neg(leftShoulder.toPoint(), 'x', 'z'),
                le = MM.neg(leftElbow.toPoint(), 'x', 'z'),
                lh = MM.neg(leftWrist.toPoint(), 'x', 'z'),
                rs = MM.neg(rightShoulder.toPoint(), 'x', 'z'),
                re = MM.neg(rightElbow.toPoint(), 'x', 'z'),
                rh = MM.neg(rightWrist.toPoint(),'x','z');
            Point3D ls1axis = new Point3D(1, 0, 0);
            Point3D ls2axis = MM.cross(MM.sub(le, ls), ls1axis);
            Point3D le2axis = MM.cross(ls2axis, MM.sub(lh, le));
            angles.leftShoulder1 = MM.RadToDeg(computeAngle(ls2axis, new Point3D(0, 0, 1), ls1axis));
            angles.leftShoulder2 = MM.RadToDeg(computeAngle(MM.cross(ls1axis, ls2axis), MM.sub(le, ls), ls2axis));
            angles.leftElbow2 = MM.RadToDeg(computeAngle(MM.sub(le, ls), MM.cross(le2axis, ls2axis), ls2axis));
            angles.leftElbow1 = MM.RadToDeg(computeAngle(MM.sub(lh, le), MM.cross(le2axis, ls2axis), le2axis));
            Point3D rs1axis = new Point3D(1, 0, 0);
            Point3D rs2axis = MM.cross(MM.sub(re, rs), rs1axis);
            Point3D re2axis = MM.cross(rs2axis, MM.sub(rh, re));
            angles.rightShoulder1 = MM.RadToDeg(computeAngle(rs2axis, new Point3D(0, 0, 1), rs1axis));
            angles.rightShoulder2 = MM.RadToDeg(-computeAngle(MM.cross(rs1axis, rs2axis), MM.sub(re, rs), rs2axis));
            angles.rightElbow2 = MM.RadToDeg(-computeAngle(MM.sub(re, rs), MM.cross(re2axis, rs2axis), rs2axis));
            angles.rightElbow1 = MM.RadToDeg(computeAngle(MM.sub(rh, re), MM.cross(re2axis, rs2axis), re2axis));
        }
        private Vector3D diff(Vector3D v1, Vector3D v2)
        {
            // simply return the difference of two vectors
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }     

        private Vector3D getVector(Skeleton skeleton, JointType joint)
        {
            //create the vector form the joint data
            return new Vector3D(skeleton.Joints[joint].Position.X,
                    skeleton.Joints[joint].Position.Y,
                    skeleton.Joints[joint].Position.Z);

        }
    }
}

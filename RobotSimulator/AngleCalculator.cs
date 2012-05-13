using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace RobotSimulator
{
    class AngleCalculator
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
                updateRightShoulder();
                updateRightElbow();
                updateLeftShoulder();
                updateLeftElbow();
            }
        }

        public AngleSet getAngles()
        {
            // return all the angles
            return angles;
        }

        private void updateLeftElbow()
        {
            // not implemented yet
        }

        private void updateLeftShoulder()
        {
            // not implemented yet
        }

        private void updateRightElbow()
        {
            // not implemented yet
        }
        private void updateRightShoulder()
        {
            // this code contains various adjustments to get the correct angles for moving the right shoulder
            Vector3D rightShoulderToElbow = diff(rightElbow, rightShoulder);

            Double rightShoulderPhi = 90 + (((Math.Atan(rightShoulderToElbow.Y / rightShoulderToElbow.X)) / Math.PI) * 180);
            Double rightShoulderTheta = (((Math.Acos(rightShoulderToElbow.Z / rightShoulderToElbow.Length)) / Math.PI) * 180) - 90;

            if (rightShoulder.X > rightElbow.X)
            {
                rightShoulderPhi = 180 + rightShoulderPhi;
            }

            if (rightShoulderPhi > 90)
            {
                rightShoulderPhi = 180 - rightShoulderPhi;
            }

            if (rightShoulder.Y < rightElbow.Y)
            {
                rightShoulderTheta = 180 - rightShoulderTheta;
            }

            if ((rightShoulder.X > rightElbow.X) && (rightShoulder.Y > rightElbow.Y))
            {
                rightShoulderTheta = rightShoulderTheta + 180;
            }

            angles.rightShoulder1 = rightShoulderTheta;
            angles.rightShoulder2 = rightShoulderPhi;
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

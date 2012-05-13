using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Restrictor;
using RobotSimulator.Utility;

namespace RobotSimulator.Model
{
    public class AngleAdapter : I_AngleListener
    {
        Portable robot;
        MotorManager manager;
        PositionCalculator posCalc;
        const double EPSILON = .001;
        const double INCREMENT = Math.PI / 36;
        public AngleAdapter(Portable robot)
        {
            this.robot = robot;
            manager = robot.motors();
            posCalc = robot.positions();
        }
        private double convert(int? angle)
        {
            if (angle == null)
                return 0;
            return ((int)angle) * Math.PI * 2 / 255;
        }
        private double[] convert(AnglePositions angles)
        {
            return new double[]{
                convert(angles.LeftShoulderAlong),
                convert(angles.LeftShoulderOut),
                convert(angles.LeftElbowAlong),
                convert(angles.LeftElbowOut),
                convert(angles.RightShoulderAlong),
                convert(angles.RightShoulderOut),
                convert(angles.RightElbowAlong),
                convert(angles.RightElbowOut)
            };
        }
        private int round(double angle)
        {
            return (int)(angle * 255 / Math.PI / 2 + .5);
        }
        private AnglePositions convert(double[] angles)
        {
            AnglePositions ap = new AnglePositions();
            ap.LeftShoulderAlong = round(angles[0]);
            ap.LeftShoulderOut = round(angles[1]);
            ap.LeftElbowAlong = round(angles[2]);
            ap.LeftElbowOut = round(angles[3]);
            ap.RightShoulderAlong = round(angles[4]);
            ap.RightShoulderOut = round(angles[5]);
            ap.RightElbowAlong = round(angles[6]);
            ap.RightElbowOut = round(angles[7]);
            return ap;
        }
        public void kinectAngles(AnglePositions angles)
        {
            if (!ready)
                return;
            double[] prevAngles = convert(CollisionRestrictor.INSTANCE.getRealAngles());
            Point3D[] target = computeKeyPositions(convert(angles));
            double[] fds = new double[8];
            for (int x = 0; x < 8; x++)
                fds[x] = finiteDifference(prevAngles, x, target);
            double lSquared = 0;
            for (int x = 0; x < 8; x++)
                lSquared += fds[x] * fds[x];
            if (lSquared == 0)
                return;
            lSquared = -Math.Sqrt(lSquared);
            for (int x = 0; x < 8; x++)
                fds[x] = prevAngles[x] + fds[x] * INCREMENT / lSquared;
            if (computeError(fds, target) <= computeError(prevAngles, target))
                CollisionRestrictor.INSTANCE.commitAngles(convert(fds));
        }
        private double finiteDifference(double[] angles, int index, Point3D[] target)
        {
            double[] a = (double[])angles.Clone();
            a[index] += EPSILON;
            double[] b = (double[])angles.Clone();
            b[index] -= EPSILON;
            return (computeError(a,target)-computeError(b,target))/EPSILON/2;
        }
        private double computeError(double[] angles, Point3D[] target)
        {
            return computeError(angles[0], angles[1], angles[2], angles[3], angles[4], angles[5], angles[6], angles[7], target);
        }
        private double computeError(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2, Point3D[] target)
        {
            return computeTargetError(ls1, ls2, le1, le2, rs1, rs2, re1, re2, target)
                + computeCollisionError(ls1, ls2, le1, le2, rs1, rs2, re1, re2)
                + computeSanityError(ls1, ls2, le1, le2, rs1, rs2, re1, re2);
        }
        private double computeTargetError(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2, Point3D[] target)
        {
            Point3D[] current = computeKeyPositions(ls1, ls2, le1, le2, rs1, rs2, re1, re2);
            double error = 0;
            for (int x = 0; x < target.Length; x++)
                error += MoreMaths.dSquared(target[x], current[x]);
            return error;
        }
        private Point3D[] computeKeyPositions(double[] angles)
        {
            return computeKeyPositions(angles[0], angles[1], angles[2], angles[3], angles[4], angles[5], angles[6], angles[7]);
        }
        private Point3D[] computeKeyPositions(double? ls1, double? ls2, double? le1, double? le2,
            double? rs1, double? rs2, double? re1, double? re2)
        {
            return new Point3D[]{posCalc.getLeftElbowCentreCoords(ls1, ls2, null),
                posCalc.getLeftShoulderCentreCoords(null),
                posCalc.getLeftHandCoords(le1, le2, ls1, ls2, null, new Point3D(0, 0, 0)),
                posCalc.getRightElbowCentreCoords(rs1, rs2, null),
                posCalc.getRightShoulderCentreCoords(null),
                posCalc.getRightHandCentreCoords(re1, re2, rs1, rs2, null, new Point3D(0, 0, 0)),
                posCalc.getBaseCoords(new Point3D(0, 0, 0)),
                posCalc.getHeadCoords(null, null, new Point3D(0, 0, 0))};
        }
        private double computeCollisionError(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2)
        {
            double torsoRadius = robot.getSize(Portable.CHEST,Portable.WIDTH);
            double armRadius = robot.getSize(Portable.LEFTUPPERARM,Portable.WIDTH);
            double forearmRadius = robot.getSize(Portable.LEFTLOWERARM,Portable.WIDTH);
            Point3D[] keyPoints = computeKeyPositions(ls1, ls2, le1, le2, rs1, rs2, re1, re2);
            double error = 0;
            foreach (double d in CollisionDetector.getDistances(keyPoints, torsoRadius, armRadius, forearmRadius))
                error += cf(d);
            return error;
        }
        private double computeSanityError(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2)
        {
            double s1min = 0,
                s1max = pi / 2,
                s2min = -pi / 2,
                s2max = pi / 2,
                e1min = -pi / 2,
                e1max = pi / 2,
                e2min = -pi / 2,
                e2max = -pi / 2;//TODO these will presumably come from Callum?
            return bf(-s1min + ls1)
                + bf(s1max - ls1)
                + bf(-s2min + ls2)
                + bf(s2max - ls2)
                + bf(-e1min + le1)
                + bf(e1max - le1)
                + bf(-e2min + le2)
                + bf(e2max - le2)
                + bf(-s1min + rs1)
                + bf(s1max - rs1)
                + bf(-s2min + rs2)
                + bf(s2max - rs2)
                + bf(-e1min + re1)
                + bf(e1max - re1)
                + bf(-e2min + re2)
                + bf(e2max - re2);
        }
        private double bf(double overlap)
        {
            return Math.Exp(-10 * overlap);
        }
        private double cf(double overlap)
        {
            return Math.Exp(-20 * overlap);
        }
        const double pi = Math.PI;
        private Boolean ready = false;
        public void robotReady()
        {
            ready = true;
        }
        public void robotStopped()
        {
            ready = false;
        }
    }
}
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
        const double INCREMENT = Math.PI / 180 * 10;
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
            return ((int)angle) * Math.PI / 180;
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
            return (int)(angle * 360 / Math.PI / 2 + .5);
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
        AnglePositions lastReturned = newAnglePositions();
        LinkedList<AnglePositions> list = new LinkedList<AnglePositions>();
        private static AnglePositions newAnglePositions()
        {
            AnglePositions a = new AnglePositions();
            a.LeftElbowAlong = 0;
            a.LeftElbowOut = 0;
            a.LeftShoulderAlong = 0;
            a.LeftShoulderOut = 0;
            a.RightElbowAlong = 0;
            a.RightElbowOut = 0;
            a.RightShoulderAlong = 0;
            a.RightShoulderOut = 0;
            return a;
        }
        private void commit(double[] angles)
        {
            list.AddLast(convert(angles));
            if (list.Count > 5)
                list.RemoveFirst();
            lastReturned=newAnglePositions();
            foreach(AnglePositions a in list)
            {
                lastReturned.LeftElbowAlong += a.LeftElbowAlong;
                lastReturned.LeftElbowOut += a.LeftElbowOut;
                lastReturned.LeftShoulderAlong += a.LeftShoulderAlong;
                lastReturned.LeftShoulderOut += a.LeftShoulderOut;
                lastReturned.RightElbowAlong += a.RightElbowAlong;
                lastReturned.RightElbowOut += a.RightElbowOut;
                lastReturned.RightShoulderAlong += a.RightShoulderAlong;
                lastReturned.RightShoulderOut += a.RightShoulderOut;
            }
            lastReturned.LeftElbowAlong /= list.Count;
            lastReturned.LeftElbowOut /= list.Count;
            lastReturned.LeftShoulderAlong /= list.Count;
            lastReturned.LeftShoulderOut /= list.Count;
            lastReturned.RightElbowAlong /= list.Count;
            lastReturned.RightElbowOut /= list.Count;
            lastReturned.RightShoulderAlong /= list.Count;
            lastReturned.RightShoulderOut /= list.Count;
            CollisionRestrictor.INSTANCE.commitAngles(lastReturned);
        }
        public void kinectAngles(AnglePositions angles)
        {
            //if (!ready)
            //    return;
            double[] prevAngles = convert(CollisionRestrictor.INSTANCE.getRealAngles());
            prevAngles = convert(lastReturned);
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
            double[] perturbation = perturb(target, prevAngles);
            double pError = computeError(perturbation, target);
            double fdError = computeError(fds, target);
            double prevError = computeError(prevAngles, target);
            if (fdError <= prevError && fdError <= pError)
                commit(fds);
            else if (pError <= prevError)
                commit(perturbation);
        }
        private static Random random=new Random();
        private double[] perturb(Point3D[] target, double[] angles)
        {
            double[] minPerturbation = angles;
            double[] perturbation = new double[8];
            double minError = Double.MaxValue;
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 8; y++)
                    perturbation[y] = random.NextDouble() - .5;
                double lSquared = 0;
                for (int y = 0; y < 8; y++)
                    lSquared += perturbation[y] * perturbation[y];
                if (lSquared == 0)
                    continue;
                lSquared = Math.Sqrt(lSquared);
                for (int y = 0; y < 8; y++)
                    perturbation[y] = angles[y] + perturbation[y] * (EPSILON / lSquared);
                double error = computeError(perturbation, target);
                if (error < minError)
                {
                    minError = error;
                    minPerturbation = perturbation;
                }
            }
            return minPerturbation;
        }
        private double finiteDifference(double[] angles, int index, Point3D[] target)
        {
            double[] a = (double[])angles.Clone();
            a[index] += EPSILON;
            double[] b = (double[])angles.Clone();
            b[index] -= EPSILON;
            double e1 = computeError(a, target), e2 = computeError(b, target);
            return (e1-e2)/EPSILON/2;
        }
        private double computeError(double[] angles, Point3D[] target)
        {
            return computeError(angles[0], angles[1], angles[2], angles[3], angles[4], angles[5], angles[6], angles[7], target);
        }
        private double computeError(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2, Point3D[] target)
        {
            double targetError = computeTargetError(ls1, ls2, le1, le2, rs1, rs2, re1, re2, target),
                collisionError = computeCollisionError(ls1, ls2, le1, le2, rs1, rs2, re1, re2),
                sanityError =  computeSanityError(ls1, ls2, le1, le2, rs1, rs2, re1, re2);
            return targetError + collisionError + sanityError;
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
        private Point3D[] computeKeyPositions(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2)
        {
            ls1 = MoreMaths.RadToDeg(ls1);
            ls2 = MoreMaths.RadToDeg(ls2);
            le1 = MoreMaths.RadToDeg(le1);
            le2 = MoreMaths.RadToDeg(le2);
            rs1 = MoreMaths.RadToDeg(rs1);
            rs2 = MoreMaths.RadToDeg(rs2);
            re1 = MoreMaths.RadToDeg(re1);
            re2 = MoreMaths.RadToDeg(re2);
            return new Point3D[]{
                posCalc.getLeftElbowCentreCoords(ls1, ls2, null),
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
            double torsoRadius = robot.getSize(Portable.CHEST,Portable.WIDTH) / 2;
            double armRadius = robot.getSize(Portable.LEFTUPPERARM,Portable.WIDTH) / 2;
            double forearmRadius = robot.getSize(Portable.LEFTLOWERARM,Portable.WIDTH) / 2;
            Point3D[] keyPoints = computeKeyPositions(ls1, ls2, le1, le2, rs1, rs2, re1, re2);
            double error = 0;
            foreach (double d in CollisionDetector.getDistances(keyPoints, torsoRadius, armRadius, forearmRadius))
                error += cf(d);
            return error;
        }
        private double computeSanityError(double ls1, double ls2, double le1, double le2,
            double rs1, double rs2, double re1, double re2)
        {
            double s1min = -100 * pi,
                s1max = 100 * pi,
                s2min = 0,
                s2max = pi,
                e1min = 0,
                e1max = 2 * pi / 3,
                e2min = -pi / 2,
                e2max = pi / 2;//TODO these will presumably come from Callum?
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
            return Math.Exp(-0 * overlap);
        }
        private double cf(double overlap)
        {
            return Math.Exp(-10 * overlap);
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
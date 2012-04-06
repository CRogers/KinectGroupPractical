using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace RobotSimulator.Model
{
    class Transforms
    {
        //You should not have access to this constructor.
        private Transforms()
        {
        }
        //Given a 3d object, it will apply the transform to it and return the new object. Note that it will not
        //overwrite the objects current transformation:
        public static Model3DGroup applyTransform(Model3DGroup obj, Transform3D transform)
        {
            Model3DGroup newObj = new Model3DGroup();
            newObj.Children.Add(obj);
            newObj.Transform = transform;
            return newObj;
        }

        //Create a rotation transformation about the specified axis by the specified number of degrees anti-clockwise:
        public static RotateTransform3D makeAxisTransform(Axis axis, double angle) {
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            switch (axis)
            {
                case Axis.X:
                    myAxisAngleRotation3d.Axis = new Vector3D(1, 0, 0);
                    break;
                case Axis.Y:
                    myAxisAngleRotation3d.Axis = new Vector3D(0, 1, 0);
                    break;
                case Axis.Z:
                    myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 1);
                    break;
                default:
                    throw new ArgumentException("Cannot rotate about unrecognised axis. ", "axis");
            }
            myAxisAngleRotation3d.Angle = angle; //rotate anticlockwise by the specified angle
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            return myRotateTransform3D;
        }
        //Create a rotation transformation about the specified axis by the specified number of degrees anti-clockwise:
        public static RotateTransform3D makeAxisTransform(Vector3D axis, double angle)
        {
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = axis;
            myAxisAngleRotation3d.Angle = angle; //rotate anticlockwise by the specified angle
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;
            return myRotateTransform3D;
        }
        //Create a scaling transformation. The first argument scales in x, the next in y and the last in z:
        public static ScaleTransform3D makeScaleTransform(double xScale, double yScale, double zScale)
        {
            ScaleTransform3D myScaleTransform3D = new ScaleTransform3D();
            myScaleTransform3D.ScaleX = xScale;
            myScaleTransform3D.ScaleY = yScale;
            myScaleTransform3D.ScaleZ = zScale;
            return myScaleTransform3D;
        }
        //Create a translation transformation. The first argument translates in x, the next in y and the last in z:
        public static TranslateTransform3D makeTranslateTransform(double xTrans, double yTrans, double zTrans)
        {
            TranslateTransform3D myTranslateTransform3D = new TranslateTransform3D();
            myTranslateTransform3D.OffsetX = xTrans;
            myTranslateTransform3D.OffsetY = yTrans;
            myTranslateTransform3D.OffsetZ = zTrans;
            return myTranslateTransform3D;
        }

    }
}

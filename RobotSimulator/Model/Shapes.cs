using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace RobotSimulator.Model
{
    //This class provides static methods for producing models for shapes:
    class Shapes
    {
        //You should not have access to the constructor:
        private Shapes()
        {
        }

        //Similar to combine, but this time the first argument becomes the parent
        public static void addChild(Model3DGroup parent, Model3DGroup child)
        {
            parent.Children.Add(child);
        }

        //Join two shapes together to form one combined shape (AKA group):
        //Note that both shapes are children of the returned shape:
        public static Model3DGroup combine(Model3DGroup shape1, Model3DGroup shape2)
        {
            Model3DGroup parent = new Model3DGroup();
            parent.Children.Add(shape1);
            parent.Children.Add(shape2);
            return parent;
        }

        //Creates a cuboid centred about the origin. The first double is the length down the x axis. The second
        //is the length down the y axis. The third is the length down the z axis.
        //It will also colour the cube with the specified colour
        public static Model3DGroup makeCuboid(double xLength, double yLength, double zLength, Color colour)
        {
            Model3DGroup cuboid = new Model3DGroup();
            Point3D p0 = new Point3D(-xLength / 2, -yLength / 2, -zLength / 2);
            Point3D p1 = new Point3D(xLength / 2, -yLength / 2, -zLength / 2);
            Point3D p2 = new Point3D(xLength / 2, -yLength / 2, zLength / 2);
            Point3D p3 = new Point3D(-xLength / 2, -yLength / 2, zLength / 2);
            Point3D p4 = new Point3D(-xLength / 2, yLength / 2, -zLength / 2);
            Point3D p5 = new Point3D(xLength / 2, yLength / 2, -zLength / 2);
            Point3D p6 = new Point3D(xLength / 2, yLength / 2, zLength / 2);
            Point3D p7 = new Point3D(-xLength / 2, yLength / 2, zLength / 2);
            //front side triangles
            cuboid.Children.Add(Shapes.makeTriangle(p3, p2, p6, colour));
            cuboid.Children.Add(Shapes.makeTriangle(p3, p6, p7, colour));
            //right side triangles
            cuboid.Children.Add(Shapes.makeTriangle(p2, p1, p5, colour));
            cuboid.Children.Add(Shapes.makeTriangle(p2, p5, p6, colour));
            //back side triangles
            cuboid.Children.Add(Shapes.makeTriangle(p1, p0, p4, colour));
            cuboid.Children.Add(Shapes.makeTriangle(p1, p4, p5, colour));
            //left side triangles
            cuboid.Children.Add(Shapes.makeTriangle(p0, p3, p7, colour));
            cuboid.Children.Add(Shapes.makeTriangle(p0, p7, p4, colour));
            //top side triangles
            cuboid.Children.Add(Shapes.makeTriangle(p7, p6, p5, colour));
            cuboid.Children.Add(Shapes.makeTriangle(p7, p5, p4, colour));
            //bottom side triangles
            cuboid.Children.Add(Shapes.makeTriangle(p2, p3, p0, colour));
            cuboid.Children.Add(Shapes.makeTriangle(p2, p0, p1, colour));
            return cuboid;
        }

        //Creates a cube centred about the origin with the specified diameter.
        public static Model3DGroup makeCube(double diameter, Color colour)
        {
            Model3DGroup cube = new Model3DGroup();
            Point3D p0 = new Point3D(-diameter / 2, -diameter / 2, -diameter / 2);
            Point3D p1 = new Point3D(diameter / 2, -diameter / 2, -diameter / 2);
            Point3D p2 = new Point3D(diameter / 2, -diameter / 2, diameter / 2);
            Point3D p3 = new Point3D(-diameter / 2, -diameter / 2, diameter / 2);
            Point3D p4 = new Point3D(-diameter / 2, diameter / 2, -diameter / 2);
            Point3D p5 = new Point3D(diameter / 2, diameter / 2, -diameter / 2);
            Point3D p6 = new Point3D(diameter / 2, diameter / 2, diameter / 2);
            Point3D p7 = new Point3D(-diameter / 2, diameter / 2, diameter / 2);
            //front side triangles
            cube.Children.Add(Shapes.makeTriangle(p3, p2, p6, colour));
            cube.Children.Add(Shapes.makeTriangle(p3, p6, p7, colour));
            //right side triangles
            cube.Children.Add(Shapes.makeTriangle(p2, p1, p5, colour));
            cube.Children.Add(Shapes.makeTriangle(p2, p5, p6, colour));
            //back side triangles
            cube.Children.Add(Shapes.makeTriangle(p1, p0, p4, colour));
            cube.Children.Add(Shapes.makeTriangle(p1, p4, p5, colour));
            //left side triangles
            cube.Children.Add(Shapes.makeTriangle(p0, p3, p7, colour));
            cube.Children.Add(Shapes.makeTriangle(p0, p7, p4, colour));
            //top side triangles
            cube.Children.Add(Shapes.makeTriangle(p7, p6, p5, colour));
            cube.Children.Add(Shapes.makeTriangle(p7, p5, p4, colour));
            //bottom side triangles
            cube.Children.Add(Shapes.makeTriangle(p2, p3, p0, colour));
            cube.Children.Add(Shapes.makeTriangle(p2, p0, p1, colour));
            return cube;
        }

        //Create a triangle given three 3D points:
        public static Model3DGroup makeTriangle(Point3D p0, Point3D p1, Point3D p2, Color colour)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Vector3D normal = Shapes.CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(
                new SolidColorBrush(colour));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup triangle = new Model3DGroup();
            triangle.Children.Add(model);
            return triangle;
        }

        //Create a default material of the specified colour:
        public static MaterialGroup getSurfaceMaterial(Color colour)
        {
            var materialGroup = new MaterialGroup();
            var emmMat = new EmissiveMaterial(new SolidColorBrush(colour));
            materialGroup.Children.Add(emmMat);
            materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(colour)));
            var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
            materialGroup.Children.Add(specMat);
            return materialGroup;
        }

        //Calulate the normal for a given triangle:
        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(
                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(
                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
    }
}

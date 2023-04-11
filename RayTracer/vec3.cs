using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RayTracer
{
    internal class Vec3
    {
        public double[] e;

        public Vec3()
        {
            e = new double[] { 0, 0, 0 };
        }

        public Vec3(double e0, double e1, double e2)
        {
            e = new double[] { e0, e1, e2 };
        }

        public double x { get { return e[0]; } }
        public double y { get { return e[1]; } }
        public double z { get { return e[2]; } }

        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v.e[0], -v.e[1], -v.e[2]);
        }

        public static Vec3 operator +(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vec3 operator -(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vec3 operator *(Vec3 v, double t)
        {
            return new Vec3(v.x * t, v.y * t, v.z * t);
        }

        public static Vec3 operator *(double t, Vec3 v)
        {
            return v * t;
        }

        public static Vec3 operator /(Vec3 v, double t)
        {
            return v * (1 / t);
        }

        public double Length()
        {
            return Math.Sqrt(Length_squared());
        }

        public double Length_squared()
        {
            return x * x + y * y + z * z;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", x, y, z);
        }

        // vec3 Utility Functions

        public Vec3 Unit_vector()
        {
            Vec3 temp = new Vec3(x, y, z);
            return temp / temp.Length();
        }

        public double Dot(Vec3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public Vec3 Cross(Vec3 v)
        {
            return new Vec3(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
        }
    }
}

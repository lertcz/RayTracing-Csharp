using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RayTracer
{
    internal class vec3
    {
        public double[] e;

        public vec3()
        {
            e = new double[] { 0, 0, 0 };
        }

        public vec3(double e0, double e1, double e2)
        {
            e = new double[] { e0, e1, e2 };
        }

        public double x { get { return e[0]; } }
        public double y { get { return e[1]; } }
        public double z { get { return e[2]; } }

        public static vec3 operator -(vec3 v)
        {
            return new vec3(-v.e[0], -v.e[1], -v.e[2]);
        }

        public static vec3 operator +(vec3 v1, vec3 v2)
        {
            return new vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static vec3 operator -(vec3 v1, vec3 v2)
        {
            return new vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static vec3 operator *(vec3 v, double t)
        {
            return new vec3(v.x * t, v.y * t, v.z * t);
        }

        public static vec3 operator *(double t, vec3 v)
        {
            return v * t;
        }

        public static vec3 operator /(vec3 v, double t)
        {
            return v * (1 / t);
        }

        public double length()
        {
            return Math.Sqrt(length_squared());
        }

        public double length_squared()
        {
            return x * x + y * y + z * z;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", x, y, z);
        }

        // vec3 Utility Functions

        public vec3 unit_vector()
        {
            vec3 temp = new vec3(x, y, z);
            return temp / temp.length();
        }

        public double dot(vec3 u, vec3 v)
        {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }

        public vec3 cross(vec3 u, vec3 v)
        {
            return new vec3(u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x);
        }
    }
}

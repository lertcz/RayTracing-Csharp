using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RayTracer
{
    internal class Vec3
    {
        public double[] e;
        private readonly static Random random = new Random();

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

        public static Vec3 operator *(Vec3 u, Vec3 v)
        {
            return new Vec3(u.x * v.x, u.y * v.y, u.z * v.z);
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
            return Math.Sqrt(LengthSquared());
        }

        public double LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }

        // vec3 Utility Functions
        public Vec3 UnitVector()
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

        public static Vec3 Random()
        {
            return new Vec3(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }
        
        public static Vec3 Random(double min, double max)
        {
            return new Vec3(random.NextDouble(min, max), random.NextDouble(min, max), random.NextDouble(min, max));
        }

        /*
        public static Vec3 RandomInUnitSphere()
        {
            while (true)
            {
                Vec3 point = Random(-1, 1);
                if (point.LengthSquared() >= 1) continue;
                return point;
            }
        }*/
        public static Vec3 RandomInUnitSphere()
        {
            int seed = Guid.NewGuid().GetHashCode();
            Random rnd = new Random(seed);

            double u = 2.0 * rnd.NextDouble() - 1.0; // Random value in the range [-1, 1]
            double theta = 2.0 * Math.PI * rnd.NextDouble(); // Random angle in the range [0, 2π]
            double r = Math.Sqrt(1.0 - u * u); // Calculate the radial distance

            double x = r * Math.Cos(theta);
            double y = r * Math.Sin(theta);
            double z = u;

            return new Vec3(x, y, z);
        }

        public static Vec3 RandomUnitVector()
        {
            return RandomInUnitSphere().UnitVector();
        }

        public static Vec3 RandomInHemisphere(Vec3 normal) {
            Vec3 in_unit_sphere = RandomInUnitSphere();
            if (in_unit_sphere.Dot(normal) > 0.0) // In the same hemisphere as the normal
                return in_unit_sphere;
            else
                return -in_unit_sphere;
        }

        public bool NearZero()
        {
            // Return true if the vector is close to zero in all dimensions.
            double limit = 1e-8;
            return (Math.Abs(x) < limit) && (Math.Abs(y) < limit) && (Math.Abs(z) < limit);
        }

        public static Vec3 Reflect(Vec3 v, Vec3 n)
        {
            return v - 2 * v.Dot(n) * n;
        }

        public static Vec3 Refract(Vec3 uv, Vec3 n, double etaiOverEtat)
        {
            double cosTheta = Math.Min(-uv.Dot(n), 1.0);
            Vec3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
            Vec3 rOutParallel = -Math.Sqrt(Math.Abs(1.0 - rOutPerp.LengthSquared())) * n;
            return rOutPerp + rOutParallel;

        }

        /*public static Vec3 RandomInUnitDisk()
        {
            while (true)
            {
                Vec3 p = new Vec3(random.NextDouble(-1, 1), random.NextDouble(-1, 1), 0);
                if (p.Length() >= 1) continue;
                return p;
            }
        }*/

        public static Vec3 RandomInUnitDisk()
        {
            double radius = Math.Sqrt(random.NextDouble());
            double theta = 2.0 * Math.PI * random.NextDouble();
            return new Vec3(radius * Math.Cos(theta), radius * Math.Sin(theta), 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class AABB
    {
        public Vec3 Minimum { get; set; }
        public Vec3 Maximum { get; set; }

        public AABB(Vec3 a, Vec3 b)
        {
            Minimum = a;
            Maximum = b;
        }

        public static AABB SurroundingBox(AABB box0, AABB box1)
        {
            var small = new Vec3(
                Math.Min(box0.Minimum.x, box1.Minimum.x),
                Math.Min(box0.Minimum.y, box1.Minimum.y),
                Math.Min(box0.Minimum.z, box1.Minimum.z));

            var big = new Vec3(
                Math.Max(box0.Maximum.x, box1.Maximum.x),
                Math.Max(box0.Maximum.y, box1.Maximum.y),
                Math.Max(box0.Maximum.z, box1.Maximum.z));

            return new AABB(small, big);
        }

        // Axis-aligned bounding box hit function by Andrew Kensler at Pixar
        public bool Hit(Ray r, double tMin, double tMax)
        {
            for (int a = 0; a < 3; a++)
            {
                double invD = 1.0 / r.Direction.val[a];
                double t0 = (Minimum.val[a] - r.Origin.val[a]) * invD;
                double t1 = (Maximum.val[a] - r.Origin.val[a]) * invD;

                if (invD < 0.0)
                {
                    // Swap t0 and t1
                    (t1, t0) = (t0, t1);
                }

                tMin = t0 > tMin ? t0 : tMin;
                tMax = t1 < tMax ? t1 : tMax;

                if (tMax <= tMin)
                    return false;
            }

            return true;
        }
    }
}

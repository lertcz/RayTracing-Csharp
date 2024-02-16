using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Box : Hittable
    {
        private readonly Vec3 boxMin;
        private readonly Vec3 boxMax;
        private readonly HittableList sides;

        public Box(Vec3 p0, Vec3 p1, Material material)
        {
            boxMin = p0;
            boxMax = p1;

            sides = new HittableList();

            sides.Add(new XYRect(p0.x, p1.x, p0.y, p1.y, p1.z, material));
            sides.Add(new XYRect(p0.x, p1.x, p0.y, p1.y, p0.z, material));

            sides.Add(new XZRect(p0.x, p1.x, p0.z, p1.z, p1.y, material));
            sides.Add(new XZRect(p0.x, p1.x, p0.z, p1.z, p0.y, material));

            sides.Add(new YZRect(p0.y, p1.y, p0.z, p1.z, p1.x, material));
            sides.Add(new YZRect(p0.y, p1.y, p0.z, p1.z, p0.x, material));
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            return sides.Hit(r, tMin, tMax, ref rec);
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            outputBox = new AABB(boxMin, boxMax);
            return true;
        }
    }
}

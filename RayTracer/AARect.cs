using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class AARect
    {
        
    }
    class XYRect : Hittable
    {
        public double x0, x1, y0, y1, k;
        public Material Material;

        public XYRect(double x0, double x1, double y0, double y1, double k, Material material)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.y0 = y0;
            this.y1 = y1;
            this.k = k;
            Material = material;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            rec = null;

            double t = (k - r.Origin.z) / r.Direction.z;
            if (t < tMin || t > tMax)
                return false;

            double x = r.Origin.x + t * r.Direction.x;
            double y = r.Origin.y + t * r.Direction.y;

            if (x < x0 || x > x1 || y < y0 || y > y1)
                return false;

            rec = new HitRecord
            {
                U = (x - x0) / (x1 - x0),
                V = (y - y0) / (y1 - y0),
                T = t,
                Material = Material,
                P = r.At(t),
            };
            Vec3 outwardNormal = new Vec3(0, 0, 1);
            rec.SetFaceNormal(r, outwardNormal);

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            // The bounding box must have non-zero width in each dimension, so pad the Z dimension a small amount.
            outputBox = new AABB(new Vec3(x0, y0, k - 0.0001), new Vec3(x1, y1, k + 0.0001));
            return true;
        }
    }

    class XZRect : Hittable
    {
        public double x0, x1, z0, z1, k;
        public Material Material;

        public XZRect(double x0, double x1, double z0, double z1, double k, Material material)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.z0 = z0;
            this.z1 = z1;
            this.k = k;
            Material = material;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            rec = null;

            double t = (k - r.Origin.y) / r.Direction.y;
            if (t < tMin || t > tMax)
                return false;

            double x = r.Origin.x + t * r.Direction.x;
            double z = r.Origin.z + t * r.Direction.z;

            if (x < x0 || x > x1 || z < z0 || z > z1)
                return false;

            rec = new HitRecord
            {
                U = (x - x0) / (x1 - x0),
                V = (z - z0) / (z1 - z0),
                T = t,
                Material = Material,
                P = r.At(t),
            };
            Vec3 outwardNormal = new Vec3(0, 0, 1);
            rec.SetFaceNormal(r, outwardNormal);

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            // The bounding box must have non-zero width in each dimension, so pad the Z dimension a small amount.
            outputBox = new AABB(new Vec3(x0, z0, k - 0.0001), new Vec3(x1, z1, k + 0.0001));
            return true;
        }
    }

    class YZRect : Hittable
    {
        public double y0, y1, z0, z1, k;
        public Material Material;

        public YZRect(double y0, double y1, double z0, double z1, double k, Material material)
        {
            this.y0 = y0;
            this.y1 = y1;
            this.z0 = z0;
            this.z1 = z1;
            this.k = k;
            Material = material;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            rec = null;

            double t = (k - r.Origin.x) / r.Direction.x;
            if (t < tMin || t > tMax)
                return false;

            double y = r.Origin.y + t * r.Direction.y;
            double z = r.Origin.z + t * r.Direction.z;

            if (y < y0 || y > y1 || z < z0 || z > z1)
                return false;

            rec = new HitRecord
            {
                U = (y - y0) / (y1 - y0),
                V = (z - z0) / (z1 - z0),
                T = t,
                Material = Material,
                P = r.At(t),
            };
            Vec3 outwardNormal = new Vec3(0, 0, 1);
            rec.SetFaceNormal(r, outwardNormal);

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            // The bounding box must have non-zero width in each dimension, so pad the Z dimension a small amount.
            outputBox = new AABB(new Vec3(y0, z0, k - 0.0001), new Vec3(y1, z1, k + 0.0001));
            return true;
        }
    }

}

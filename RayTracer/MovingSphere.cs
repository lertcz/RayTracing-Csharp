using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class MovingSphere : Hittable
    {
        Vec3 Center0, Center1;
        double Time0, Time1;
        double Radius;
        Material Material;

        public MovingSphere(Vec3 cen0, Vec3 cen1, double _time0, double _time1, double r, Material material)
        {
            Center0 = cen0;
            Center1 = cen1;
            Time0 = _time0;
            Time1 = _time1;
            Radius = r;
            Material = material;
        }

        public Vec3 Center(double time)
        {
            return Center0 + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0);
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            Vec3 oc = r.Origin - Center(r.Time);
            double a = r.Direction.LengthSquared();
            double halfB = oc.Dot(r.Direction);
            double c = oc.LengthSquared() - Radius * Radius;

            double discriminant = halfB * halfB - a * c;
            if (discriminant < 0) return false;
            double sqrtD = Math.Sqrt(discriminant);

            // Find the nearest root that lies in the acceptable range
            double root = (-halfB - sqrtD) / a;
            if (root < tMin || tMax < root)
            {
                root = (-halfB + sqrtD) / a;
                if (root < tMin || tMax < root) return false;
            }

            rec.T = root;
            rec.P = r.At(rec.T);
            Vec3 outwardNormal = (rec.P - Center(r.Time)) / Radius;
            rec.SetFaceNormal(r, outwardNormal);
            rec.Material = Material;

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            var box0 = new AABB(
                Center(time0) - new Vec3(Radius, Radius, Radius),
                Center(time0) + new Vec3(Radius, Radius, Radius));

            var box1 = new AABB(
                Center(time1) - new Vec3(Radius, Radius, Radius),
                Center(time1) + new Vec3(Radius, Radius, Radius));

            outputBox = AABB.SurroundingBox(box0, box1);

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Sphere : Hittable
    {
        public Vec3 Center { get; set; }
        public double Radius { get; set; }
        public Material Material { get; set; }

        public Sphere(Vec3 center, double r, Material material)
        {
            Center = center;
            Radius = r;
            Material = material;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            Vec3 oc = r.Origin - Center;
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
            Vec3 outwardNormal = (rec.P - Center) / Radius;
            rec.SetFaceNormal(r, outwardNormal);
            rec.Material = Material;

            return true;
        }
    }
}

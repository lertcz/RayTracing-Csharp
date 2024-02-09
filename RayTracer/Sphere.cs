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

        public static void GetSphereUV(Vec3 p, out double u, out double v)
        {
            // p: a given point on the sphere of radius one, centered at the origin.
            // u: returned value [0,1] of angle around the Y axis from X=-1.
            // v: returned value [0,1] of angle from Y=-1 to Y=+1.
            //     <1 0 0> yields <0.50 0.50>       <-1  0  0> yields <0.00 0.50>
            //     <0 1 0> yields <0.50 1.00>       < 0 -1  0> yields <0.50 0.00>
            //     <0 0 1> yields <0.25 0.50>       < 0  0 -1> yields <0.75 0.50>

            double theta = Math.Acos(-p.y);
            double phi = Math.Atan2(-p.z, p.x) + Math.PI;

            u = phi / (2 * Math.PI);
            v = theta / Math.PI;
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
            GetSphereUV(outwardNormal, out rec.U, out rec.V);
            rec.Material = Material;

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            outputBox = new AABB(
                Center - new Vec3(Radius, Radius, Radius),
                Center + new Vec3(Radius, Radius, Radius)
            );
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class HitRecord
    {
        public Vec3 P;
        public Vec3 Normal;
        public Material Material;
        public double T;
        public double U;
        public double V;
        public bool FrontFace;

        public void SetFaceNormal(Ray r, Vec3 outwardNormal)
        {
            FrontFace = r.Direction.Dot(outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }

    internal abstract class Hittable
    {
        public abstract bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec);
        public abstract bool BoundingBox(double time0, double time1, out AABB outputBox);
    }

    class Translate : Hittable
    {
        public Hittable ptr;
        public Vec3 offset;

        public Translate(Hittable p, Vec3 displacement)
        {
            ptr = p;
            offset = displacement;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            Ray movedR = new Ray(r.Origin - offset, r.Direction, r.Time);
            if (!ptr.Hit(movedR, tMin, tMax, ref rec))
            {
                return false;
            }

            rec.P += offset;
            rec.SetFaceNormal(movedR, rec.Normal);

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            if (!ptr.BoundingBox(time0, time1, out outputBox))
            {
                return false;
            }

            outputBox = new AABB(outputBox.Minimum + offset, outputBox.Maximum + offset);
            return true;
        }
    }

    class RotateY : Hittable
    {
        public Hittable ptr;
        public double sinTheta;
        public double cosTheta;
        public bool hasBox;
        public AABB bbox;

        public RotateY(Hittable p, double angle)
        {
            ptr = p;
            double radians = (Math.PI / 180.0) * angle; // deg 2 rad
            sinTheta = Math.Sin(radians);
            cosTheta = Math.Cos(radians);
            hasBox = ptr.BoundingBox(0, 1, out bbox);

            Vec3 min = new Vec3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vec3 max = new Vec3(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        double x = i * bbox.Maximum.x + (1 - i) * bbox.Minimum.x;
                        double y = j * bbox.Maximum.y + (1 - j) * bbox.Minimum.y;
                        double z = k * bbox.Minimum.z + (1 - k) * bbox.Minimum.z;

                        double newX = cosTheta * x + sinTheta * z;
                        double newZ = -sinTheta * x + cosTheta * z;

                        Vec3 tester = new Vec3(newX, y, newZ);

                        for (int c = 0; c < 3; c++)
                        {
                            min.val[c] = Math.Min(min.val[c], tester.val[c]);
                            max.val[c] = Math.Max(max.val[c], tester.val[c]);
                        }
                    }
                }
            }

            bbox = new AABB(min, max);
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            Vec3 origin = new Vec3(cosTheta * r.Origin.val[0] - sinTheta * r.Origin.val[2],
                                    r.Origin.val[1],
                                    cosTheta * r.Origin.val[0] + sinTheta * r.Origin.val[2]);
            Vec3 direction = new Vec3(cosTheta * r.Direction.val[0] - sinTheta * r.Direction.val[2],
                                    r.Direction.val[1],
                                    cosTheta * r.Direction.val[0] + sinTheta * r.Direction.val[2]);

            Ray rotatedRay = new Ray(origin, direction, r.Time);

            if (!ptr.Hit(rotatedRay, tMin, tMax, ref rec)) return false;

            Vec3 p = new Vec3(cosTheta * rec.P.val[0] + sinTheta * rec.P.val[2],
                                rec.P.val[1],
                                -sinTheta * rec.P.val[0] + cosTheta * rec.P.val[2]);
            Vec3 normal = new Vec3(cosTheta * rec.Normal.val[0] + sinTheta * rec.Normal.val[2],
                                rec.Normal.val[1],
                                -sinTheta * rec.Normal.val[0] + cosTheta * rec.Normal.val[2]);

            rec.P = p;
            rec.SetFaceNormal(rotatedRay, normal);  

            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            outputBox = bbox;
            return hasBox;
        }
    }
}

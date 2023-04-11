using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public struct HitRecord
    {
        Vec3 P;
        Vec3 Normal;
        double T;
        bool FrontFace;

        void SetFaceNormal(Ray r, Vec3 outwardNormal)
        {
            FrontFace = r.Direction.Dot(outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }

    internal abstract class Hittable
    {
        public abstract bool Hit(Ray r, double tMin, double tMax, HitRecord rec);
    }
}

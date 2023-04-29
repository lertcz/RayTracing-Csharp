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
    }
}

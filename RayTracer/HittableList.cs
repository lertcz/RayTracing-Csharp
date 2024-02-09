using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class HittableList : Hittable
    {
        // check later for shared pointers
        public List<Hittable> Objects = new List<Hittable> ();

        public void Clear()
        {
            Objects.Clear();
        }

        public void Add(Hittable obj)
        {
            Objects.Add(obj);
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            if (Objects.Count == 0)
            {
                outputBox = null;
                return false;
            }

            outputBox = null;
            bool firstBox = true;

            foreach (var hittableObject in Objects)
            {
                if (!hittableObject.BoundingBox(time0, time1, out var tempBox))
                {
                    outputBox = null;
                    return false;
                }

                outputBox = firstBox ? tempBox : AABB.SurroundingBox(outputBox, tempBox);
                firstBox = false;
            }

            return true;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            HitRecord tempRec = new HitRecord();
            bool hitAnything = false;
            double closestSoFar = tMax;

            foreach (var obj in Objects)
            {
                if (obj.Hit(r, tMin, closestSoFar, ref tempRec))
                {
                    hitAnything = true;
                    closestSoFar = tempRec.T;
                    rec = tempRec;
                }
            }

            return hitAnything;
        }
    }
}

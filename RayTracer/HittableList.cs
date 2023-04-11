using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class HittableList : Hittable
    {
        // check later for shared pointers
        public List<Hittable> Objects = new List<Hittable> ();

        public HittableList(Hittable obj)
        {
            Add(obj);
        }

        public void Clear()
        {
            Objects.Clear();
        }

        public void Add(Hittable obj)
        {
            Objects.Add(obj);
        }

        public bool Hit(Ray r, double tMin, double tMax, HitRecord rec)
        {
            HitRecord tempRec = new HitRecord();
            bool hitAnything = false;
            double closestSoFar = tMax;

            foreach (var obj in Objects)
            {
                if (obj.Hit(r, tMin, closestSoFar, tempRec))
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class BVH : Hittable
    {
        private Hittable Left;
        private Hittable Right;
        private AABB Box;

        public BVH(List<Hittable> srcObjects, int start, int end, double time0, double time1)
        {
            List<Hittable> objects = srcObjects.ToList();

            int axis = 1; //new Random().Next(0, 3);
            // 0 -> x, 1 -> y, 2 -> z
            Comparison<Hittable> comparator = (a, b) => (axis == 0) ? BoxXCompare(a, b)
                                                      : (axis == 1) ? BoxYCompare(a, b)
                                                      : BoxZCompare(a, b);


            int objectSpan = end - start;

            if (objectSpan == 1)
            {
                Left = Right = objects[start];
            }
            else if (objectSpan == 2)
            {
                if (comparator(objects[start], objects[start + 1]) < 0)
                {
                    Left = objects[start];
                    Right = objects[start + 1];
                }
                else
                {
                    Left = objects[start + 1];
                    Right = objects[start];
                }
            }
            else
            {
                List<Hittable> sortedSublist = objects.GetRange(start, objectSpan);
                //sortedSublist.Sort(comparator);

                //objects.RemoveRange(start, objectSpan);
                //objects.InsertRange(start, sortedSublist);

                int mid = start + objectSpan / 2;   
                Left = new BVH(objects, start, mid, time0, time1);
                Right = new BVH(objects, mid, end, time0, time1);
            }

            bool boxLeftVal = Left.BoundingBox(time0, time1, out AABB boxLeft);
            bool boxRightVal = Right.BoundingBox(time0, time1, out AABB boxRight);

            if (!boxLeftVal || !boxRightVal)
            {
                Console.Error.WriteLine("No bounding box in BVH constructor.");
            }

            Box = AABB.SurroundingBox(boxLeft, boxRight);
        }

        private static int BoxCompare(Hittable a, Hittable b, int axis)
        {
            bool boxAVal = a.BoundingBox(0, 0, out AABB boxA);
            bool boxBVal = b.BoundingBox(0, 0, out AABB boxB);

            if (!boxAVal || !boxBVal)
            {
                Console.Error.WriteLine("No bounding box in BVH constructor.");
            }

            //return boxA.Minimum.val[axis] < boxB.Minimum.val[axis];
            return boxA.Minimum.val[axis].CompareTo(boxB.Minimum.val[axis]);
        }
        private static int BoxXCompare(Hittable a, Hittable b) => BoxCompare(a, b, 0);
        private static int BoxYCompare(Hittable a, Hittable b) => BoxCompare(a, b, 1);
        private static int BoxZCompare(Hittable a, Hittable b) => BoxCompare(a, b, 2);

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord rec)
        {
            if (!Box.Hit(r, tMin, tMax))  return false;

            return Left.Hit(r, tMin, tMax, ref rec) || Right.Hit(r, tMin, tMax, ref rec);
        }

        public override bool BoundingBox(double time0, double time1, out AABB outputBox)
        {
            outputBox = Box;
            return true;
        }
    }
}

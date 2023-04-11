using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Ray
    {
        public Vec3 Origin { get; set; }
        public Vec3 Direction { get; set; }

        public Ray(Vec3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
        public Vec3 At(double t)
        {
            return Origin + t * Direction;
        }
    }
}

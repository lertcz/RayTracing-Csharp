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
        public double Time { get; set; }

        public Ray(Vec3 origin, Vec3 direction, double time = 0.0)
        {
            Origin = origin;
            Direction = direction;
            Time = time;
        }
        public Vec3 At(double t)
        {
            return Origin + t * Direction;
        }
    }
}

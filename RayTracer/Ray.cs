using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Ray
    {
        public vec3 Origin { get; set; }
        public vec3 Direction { get; set; }

        public Ray(vec3 origin, vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
        public vec3 At(double t)
        {
            return Origin + t * Direction;
        }
    }
}

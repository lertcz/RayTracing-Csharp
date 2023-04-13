using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Camera
    {
        const double AspectRatio = 16.0 / 9.0;
        private static readonly double ViewportHeight = 2.0;
        private static readonly double ViewportWidth = AspectRatio * ViewportHeight;
        private static readonly double FocalLength = 1.0;

        private static readonly Vec3 Origin = new Vec3(0, 0, 0);
        private static readonly Vec3 Horizontal = new Vec3(ViewportWidth, 0, 0);
        private static readonly Vec3 Vertical = new Vec3(0, ViewportHeight, 0);
        private static readonly Vec3 LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - new Vec3(0, 0, FocalLength);

        public Ray GetRay(double u, double v)
        {
            return new Ray(Origin, LowerLeftCorner + u * Horizontal + v * Vertical - Origin);
        }
    }
}

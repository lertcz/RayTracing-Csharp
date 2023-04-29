using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Camera
    {

        private readonly Vec3 Origin, Horizontal, Vertical, LowerLeftCorner;

        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vectorUP, double vfov, double aspectRatio)
        {
            double theta = vfov * Math.PI / 180;
            double h = Math.Tan(theta / 2);
            double ViewportHeight = 2.0 * h;
            double ViewportWidth = aspectRatio * ViewportHeight;

            Vec3 w = (lookFrom - lookAt).UnitVector();
            Vec3 u = vectorUP.Cross(w).UnitVector();
            Vec3 v = w.Cross(u);

            Origin = lookFrom;
            Horizontal = ViewportWidth * u;
            Vertical = ViewportHeight * v;
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - w;
        }

        public Ray GetRay(double s, double t)
        {
            return new Ray(Origin, LowerLeftCorner + s * Horizontal + t * Vertical - Origin);
        }
    }
}

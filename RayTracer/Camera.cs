using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Camera
    {
        private readonly static Random random = new Random();

        private readonly Vec3 Origin, Horizontal, Vertical, LowerLeftCorner;

        private readonly Vec3 w, u, v;
        private readonly double lensRadius;
        private readonly double time0, time1; // shutter open/close times

        public Camera(
            Vec3 lookFrom,
            Vec3 lookAt,
            Vec3 vectorUP,
            double vfov,
            double aspectRatio,
            double aperture,
            double focusDistance,
            double _time0 = 0.0,
            double _time1 = 0.0
        )
        {
            double theta = vfov * Math.PI / 180;
            double h = Math.Tan(theta / 2);
            double ViewportHeight = 2.0 * h;
            double ViewportWidth = aspectRatio * ViewportHeight;

            w = (lookFrom - lookAt).UnitVector();
            u = vectorUP.Cross(w).UnitVector();
            v = w.Cross(u);

            Origin = lookFrom;
            Horizontal = focusDistance * ViewportWidth * u;
            Vertical   = focusDistance * ViewportHeight * v;
            LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - focusDistance * w;

            lensRadius = aperture / 2;
            time0 = _time0;
            time1 = _time1;
        }

        public Ray GetRay(double s, double t)
        {
            Vec3 rd = lensRadius * Vec3.RandomInUnitDisk();
            Vec3 offset = u * rd.x + v * rd.y;

            return new Ray(
                Origin + offset,
                LowerLeftCorner + s * Horizontal + t * Vertical - Origin - offset,
                random.NextDouble(time0, time1)
            );
        }
    }
}

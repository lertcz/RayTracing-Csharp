using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class MovingSphere : Hittable
    {
        Vec3 Center0, Center1;
        double Time0, Time1;
        double Radius;
        Material Material;

        public MovingSphere(Vec3 cen0, Vec3 cen1, double _time0, double _time1, double r, Material material)
        {
            Center0 = cen0;
            Center1 = cen1;
            Time0 = _time0;
            Time1 = _time1;
            Radius = r;
            Material = material;
        }

        public Vec3 Center(double time)
        {
            return Center0 + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0);
        }
    }
}

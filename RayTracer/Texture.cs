using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal abstract class Texture
    {
        public abstract Vec3 Value(double u, double v, Vec3 p);
    }
    class SolidColor : Texture
    {
        public Vec3 ColorValue { get; set; }

        public SolidColor(Vec3 c)
        {
            ColorValue = c;
        }

        public SolidColor(float red, float green, float blue)
        {
            ColorValue = new Vec3(red, green, blue);
        }

        public override Vec3 Value(double u, double v, Vec3 p)
        {
            return ColorValue;
        }
    }

    class CheckerTexture : Texture
    {
        private readonly SolidColor even;
        private readonly SolidColor odd;

        public CheckerTexture(SolidColor evenTexture, SolidColor oddTexture)
        {
            even = evenTexture;
            odd = oddTexture;
        }

        public CheckerTexture(Vec3 c1, Vec3 c2)
        {
            even = new SolidColor(c1);
            odd = new SolidColor(c2);
        }

        public override Vec3 Value(double u, double v, Vec3 p)
        {
            double sines = Math.Sin(10 * p.x) * Math.Sin(10 * p.y) * Math.Sin(10 * p.z);
            return sines < 0 ? odd.Value(u, v, p) : even.Value(u, v, p);
        }
    }
}

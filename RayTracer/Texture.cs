using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Media3D;

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

    class NoiseTexture : Texture
    {
        readonly Perlin noise = new Perlin();
        readonly double Scale;

        public NoiseTexture(double scale = 1)
        {
            Scale = scale;
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            return new Vec3(1, 1, 1) * 0.5 * (1.0 + noise.Noise(Scale * p));
        }
    }

    class TurbulentNoise : Texture
    {
        readonly Perlin noise = new Perlin();
        readonly double Scale;

        public TurbulentNoise(double scale = 1)
        {
            Scale = scale;
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            return new Vec3(1, 1, 1) * noise.Turb(Scale * p);
        }
    }

    class MarbleTexture : Texture
    {
        readonly Perlin noise = new Perlin();
        readonly double Scale;

        public MarbleTexture(double scale = 1)
        {
            Scale = scale;
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            return new Vec3(1, 1, 1) * 0.5 * (1.0 + Math.Sin(Scale * p.z + 10 * noise.Turb(p)));
        }
    }

    class ImageTexture : Texture
    {
        private readonly Bitmap image;

        public ImageTexture(string filename)
        {
            image = new Bitmap(filename);
        }

        public override Vec3 Value(double u, double v, Vec3 p)
        {
            // If we have no texture data, then return solid cyan as a debugging aid.
            if (image.Height <= 0) return new Vec3(0, 1, 1);

            // Clamp input texture coordinates to [0,1] x [1,0]
            u = u.Clamp(0, 1);
            v = 1.0 - v.Clamp(0, 1);  // Flip V to image coordinates

            int i = (int)(u * image.Width);
            int j = (int)(v * image.Height);

            if (i >= image.Width) i = image.Width - 1;
            if (j >= image.Height) j = image.Height - 1;

            System.Drawing.Color pixel = image.GetPixel(i, j);

            double colorScale = 1.0 / 255.0;
            return new Vec3(colorScale * pixel.R, colorScale * pixel.G, colorScale * pixel.B);
        }
    }
}

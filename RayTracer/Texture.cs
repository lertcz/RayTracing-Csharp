using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using System.IO; // for directory location

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
        private readonly double frequency;

        public CheckerTexture(SolidColor evenTexture, SolidColor oddTexture, double frequency = 10.0)
        {
            even = evenTexture;
            odd = oddTexture;
            this.frequency = frequency;
        }

        public CheckerTexture(Vec3 c1, Vec3 c2, double frequency = 10.0)
        {
            even = new SolidColor(c1);
            odd = new SolidColor(c2);
            this.frequency = frequency;
        }

        public override Vec3 Value(double u, double v, Vec3 p)
        {
            double sines = Math.Sin(frequency * p.x) * Math.Sin(frequency * p.y) * Math.Sin(frequency * p.z);
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
        //ConcurrentDictionary<int, System.Drawing.Color> image = new ConcurrentDictionary<Tuple<int, int, Color>>(new Tuple<int, int, Color>[image_height * image_width]);
        private readonly ConcurrentDictionary<(int, int), Color> imageData = new ConcurrentDictionary<(int, int), Color>();
        private readonly int Height, Width;
        private bool pathFailed = false;

        public ImageTexture(string filename)
        {
            // TODO better loading!! mb from blender :)

            Console.WriteLine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Images\\" + filename);
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Images\\" + filename;
            Console.WriteLine(path);

            try
            {
                Bitmap tempImg = new Bitmap(path);
                Height = tempImg.Height;
                Width = tempImg.Width;
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        imageData[(w, h)] = tempImg.GetPixel(w, h);
                    }
                }
            }
            catch (ArgumentException) {
                pathFailed = true;
            }

        }

        public override Vec3 Value(double u, double v, Vec3 p)
        {
            //return new Vec3(0, 1, 1);
            
            // If we have no texture data, then return solid cyan as a debugging aid.
            if (imageData.Count <= 0 || pathFailed) return new Vec3(0, 1, 1);

            // Clamp input texture coordinates to [0,1] x [1,0]
            u = u.Clamp(0, 1);
            v = 1.0 - v.Clamp(0, 1);  // Flip V to image coordinates

            int i = (int)(u * Width);
            int j = (int)(v * Height);

            if (i >= Width) i = Width - 1;
            if (j >= Height) j = Height - 1;

            Color pixel = imageData[(i, j)];

            double colorScale = 1.0 / 255.0;
            return new Vec3(colorScale * pixel.R, colorScale * pixel.G, colorScale * pixel.B);
        }
    }
}

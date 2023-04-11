using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace RayTracer
{
    internal class Render
    {
        // Image
        const double aspect_ratio = 16.0 / 9.0;
        const int image_width = 400;
        const int image_height = (int)(image_width / aspect_ratio);

        // World
        static HittableList World;
        World.Add(new Sphere(new Vec3(0, 0, -1), 0.5));
        World.Add(new Sphere(new Vec3(0, -100.5, -1), 100));

        // Camera
        static double ViewportHeight = 2.0;
        static double ViewportWidth = aspect_ratio * ViewportHeight;
        static double FocalLength = 1.0;

        static Vec3 Origin = new Vec3(0, 0, 0);
        static Vec3 Horizontal = new Vec3(ViewportWidth, 0, 0);
        static Vec3 Vertical = new Vec3(0, ViewportHeight, 0);
        static Vec3 LowerLeftCorner = Origin - Horizontal / 2 - Vertical / 2 - new Vec3(0, 0, FocalLength);

        // Other
        public Bitmap Result;

        public System.Windows.Controls.Image Render_image { get; set; }

        public void Start()
        {
            new Thread(() =>
            {
                Bitmap image = new Bitmap(image_width, image_height);

                // Render
                Bitmap img = SingleProcess();

                Render_image.Dispatcher.Invoke(() => Render_image.Source = BitmapToImageSource(img));
                Result = img; // save Bitmap in a variable for saving into a file

            }).Start();
        }

        public Bitmap SingleProcess()
        {
            Bitmap image = new Bitmap(image_width, image_height);

            for (double y = image_height - 1; y >= 0; --y)
            {
                Debug.WriteLine("Scanlines remaining: {0}", y);
                for (double x = 0; x < image_width; ++x)
                {
                    double u = x / (image_width-1);
                    double v = y / (image_height-1);
                    Ray r = new Ray(Origin, LowerLeftCorner + u * Horizontal +  v * Vertical - Origin);
                    Vec3 Pixel_color = Ray_color(r, world);

                    image.SetPixel(
                        (int)x,
                        image_height - 1 - (int)y, // flip the image for bitmap
                        Ray_color_to_pixel(Pixel_color)
                    );
                }
            }
            Debug.WriteLine("Done");

            return image;
        }

        private Vec3 Ray_color(Ray r, Hittable world) {
            HitRecord rec;
            if (world.Hit(r, 0, double.MaxValue, rec))
            {
                return 0.5 * (rec.Normal + new Vec3(1, 1, 1));
            }
            Vec3 unit_direction = r.Direction.Unit_vector();
            double t = 0.5 * (unit_direction.y + 1.0);
            return (1.0-t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0);
        }

        private Color Ray_color_to_pixel(Vec3 color)
        {
            return Color.FromArgb(
                (int)(255.999 * color.x),
                (int)(255.999 * color.y),
                (int)(255.999 * color.z)
            );
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    
        private Bitmap StackBitmaps(List<Bitmap> Images)
        {
            Int32 width = image_width;
            Int32 height = 0;
            for (Int32 i = 0; i < Images.Count; i++)
            {
                height += Images[i].Height;
            }

            Bitmap bitmap2 = new Bitmap(width, height);
            bitmap2.SetResolution(72, 72); // <-- Set explicit resolution on bitmap2
                                           // Always put Graphics objects in a 'using' block.
            using (Graphics g = Graphics.FromImage(bitmap2))
            {
                height = 0;
                for (Int32 i = 0; i < Images.Count; i++)
                {
                    Bitmap image = Images[i];
                    image.SetResolution(72, 72); // <-- Set resolution equal to bitmap2
                    g.DrawImage(image, 0, height);
                    height += image.Height;
                }
            }
            return bitmap2;
        }
    }
}

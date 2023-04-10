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

        // Camera
        static double viewport_height = 2.0;
        static double viewport_width = aspect_ratio * viewport_height;
        static double focal_length = 1.0;

        static vec3 origin = new vec3(0, 0, 0);
        static vec3 horizontal = new vec3(viewport_width, 0, 0);
        static vec3 vertical = new vec3(0, viewport_height, 0);
        static vec3 lower_left_corner = origin - horizontal / 2 - vertical / 2 - new vec3(0, 0, focal_length);

        // Other
        public Bitmap Result;

        public System.Windows.Controls.Image Render_image { get; set; }

        public void Start()
        {
            new Thread(() =>
            {
                Bitmap image = new Bitmap(image_width, image_height);

                // Render
                //Bitmap image = MultiThreadRendering(image, 1);
                Bitmap img = SingleProcess();

                //List<Bitmap> images = new List<Bitmap>();

                //for (int i = 0; i < 128; i++)
                //{
                //    images.Add(im1);
                //}

                //Bitmap FullImage = Stack(images);
                // threads.All(t => t.IsAlive == true) // check if all threads are alive

                Render_image.Dispatcher.Invoke(() => Render_image.Source = BitmapToImageSource(img));
                Result = img; // save Bitmap in a variable for saving into a file


            }).Start();
        }

        /*public List<Thread> RowRendering(int row)
        {
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < image_height; i++)
            {
                Thread t = new Thread(() =>
                {
                    Bitmap image = new Bitmap(image_width, 1);

                    for (double x = 0; x < image_width; ++x)
                    {
                        double r = x / (image_width - 1);
                        double g = row / (image_height - 1);
                        double b = .25;

                        // interpolated colors
                        int ir = (int)(255.999 * r);
                        int ig = (int)(255.999 * g);
                        int ib = (int)(255.999 * b);

                        image.SetPixel((int)x, image_height - 1 - row, Color.FromArgb(ir, ig, ib));
                    }
                });
                t.IsBackground = true;
                t.Name = "RayTracingProcess" + i.ToString();

                threads.Add(t);
            }

            Console.WriteLine("THEADS");
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            Console.WriteLine("finished");

            return new Bitmap(image_width, image_height);
        }*/

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
                    Ray r = new Ray(origin, lower_left_corner + u*horizontal +  v*vertical - origin);
                    vec3 Pixel_color = Ray_color(r);

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

        // something here is broken
        private vec3 Ray_color(Ray r) {
            vec3 unit_direction = r.Direction.unit_vector();
            double t = 0.5 * (unit_direction.y + 1.0);
            return (1.0-t) * new vec3(1.0, 1.0, 1.0) + t * new vec3(0.5, 0.7, 1.0);
        }

        private Color Ray_color_to_pixel(vec3 color)
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

﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RayTracer
{
    internal class Render : INotifyPropertyChanged
    {
        readonly Random rnd = new Random();
        public event PropertyChangedEventHandler PropertyChanged;
        private double _renderProgress = 0;
        public double RenderProgress
        {
            get { return _renderProgress; }
            set
            {
                _renderProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RenderProgress"));
                //Debug.WriteLine("Progress {0}", value);
            }
        }

        // Image Settings
        const float aspectRatio = 16.0f / 9.0f;
        const int image_width = 400; // 400
        const int image_height = (int)(image_width / aspectRatio);
        const int SamplesPerPixel = 20; //100
        // 50 (min - 2 albedo, 3 metal, 5 glass) 
        const int MaxDepth = 10; // Maximum number of ray bounces into the scene

        // Camera Setup
        static Vec3 LookFrom, LookAt;
        double vfov = 40.0;
        readonly static Vec3 VectorUP = new Vec3(0, 1, 0);
        readonly static float distanceToFocus = 10f; //(LookFrom - LookAt).Length(); // auto focus
        static float aperture = 0.0f;
        Camera Cam; // = new Camera(LookFrom, LookAt, VectorUP, 20, aspectRatio, aperture, distanceToFocus, 0, 1);

        // Scene
        HittableList World;

        // Bitmap init
        public Bitmap Result;
        public System.Windows.Controls.Image Render_image { get; set; }

        public void Start(int scene)
        {
            switch (scene)
            {
                case 1:
                    World = Scenes.Part1RandomFinalScene();
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    aperture = 0.1f;
                    break;

                case 2:
                    World = Scenes.MotionBlurScene();
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    aperture = 0.1f; 
                    break;

                case 3:
                    World = Scenes.TwoSpheres();
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;

                case 4:
                    World = Scenes.TwoPerlinSpheres();
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;

                default:
                case 5:
                    World = Scenes.MarbleAndTurbulence();
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;
            }
            Cam = new Camera(LookFrom, LookAt, VectorUP, vfov, aspectRatio, aperture, distanceToFocus, 0, 1);

            Debug.WriteLine("Render start");
            new Thread(() =>
            {
                Bitmap image = new Bitmap(image_width, image_height);

                // Render
                //Bitmap img = SingleProcess();
                Bitmap img = MultiThread();

                Render_image.Dispatcher.Invoke(() => Render_image.Source = BitmapToImageSource(img));
                Result = img; // save Bitmap in a variable for saving into a file

            }).Start();
        }

        public Bitmap SingleProcess()
        {
            Bitmap image = new Bitmap(image_width, image_height);
            RenderProgress = 0;

            for (double y = image_height - 1; y >= 0; --y)
            {
                for (double x = 0; x < image_width; ++x)
                {
                    Vec3 PixelColor = new Vec3(0, 0, 0);
                    for (int s = 0; s < SamplesPerPixel; ++s)
                    {
                        double u = (x + rnd.NextDouble(0.0, 1.0)) / (image_width - 1);
                        double v = (y + rnd.NextDouble(0.0, 1.0)) / (image_height - 1);
                        Ray r = Cam.GetRay(u, v);
                        PixelColor += RayColor(r, World, MaxDepth);
                    }

                    image.SetPixel(
                        (int)x,
                        image_height - 1 - (int)y, // flip the image for bitmap
                        RayColorToPixel(PixelColor)
                    );
                }
                RenderProgress = Math.Round((image_height - y) / image_height * 100, 2);
            }

            return image;
        }

        public Bitmap MultiThread()
        {
            var pixels = new ConcurrentStack<Tuple<int, int, Color>>(new Tuple<int, int, Color>[image_height*image_width]);
            

            double CompletedRows = 1;
            RenderProgress = 0;


            //var options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = 10;
            Parallel.For(0, image_height, y => // for (double y = 0; y < image_height; ++y)
            {

                var localPixels = new Tuple<int, int, Color>[image_width];
                for (int x = 0; x < image_width; ++x)
                {
                    Vec3 PixelColor = new Vec3(0, 0, 0);

                    for (int s = 0; s < SamplesPerPixel; ++s)
                    {
                        double u = (x + rnd.NextDouble(0.0, 1.0)) / (image_width - 1);
                        double v = (y + rnd.NextDouble(0.0, 1.0)) / (image_height - 1);
                        Ray r = Cam.GetRay(u, v);
                        PixelColor += RayColor(r, World, MaxDepth);
                    }
                    localPixels[x] = new Tuple<int, int, Color>(image_height - 1 - y, x, RayColorToPixel(PixelColor));
                }

                pixels.PushRange(localPixels);

                CompletedRows++;
                RenderProgress = Math.Round(CompletedRows / image_height * 100, 2);

            });

            Debug.WriteLine("DONE");

            Bitmap image = new Bitmap(image_width, image_height);

            while ( ! pixels.IsEmpty)
            {

                if (pixels.TryPop(out var pixel))
                {
                    if (pixel == null)
                    {
                        break;
                    }
                    image.SetPixel(pixel.Item2, pixel.Item1, pixel.Item3);
                }
            }
            

            return image;
        }

        private Vec3 RayColor(Ray r, Hittable World, int depth)
        {
            HitRecord rec = new HitRecord();

            // If we've exceeded the ray bounce limit, no more light is gathered.
            if (depth <= 0)
                return new Vec3(0.0, 0.0, 0.0);

            if (World.Hit(r, 0.001, double.MaxValue, ref rec))
            {
                if (rec.Material.Scatter(r, ref rec, out Vec3 colorAttenuation, out Ray scattered))
                {
                    return colorAttenuation * RayColor(scattered, World, depth - 1);
                }
                return new Vec3(0.0, 0.0, 0.0);
            }
            Vec3 unit_direction = r.Direction.UnitVector();
            double t = 0.5 * (unit_direction.y + 1.0);
            return (1.0 - t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0);
        }

        /*private Vec3 RayColor(Ray r, Hittable World, int maxDepth) // sample glitch
        {

            var vec3one = new Vec3(1.0, 1.0, 1.0);
            var vec3numbers = new Vec3(0.5, 0.7, 1.0);

            Vec3 color = new Vec3(1.0, 1.0, 1.0); // Initialize with the background color
            Ray currentRay = r;

            for (int depth = maxDepth; depth > 0; depth--)
            {
                HitRecord rec = new HitRecord();

                if (World.Hit(currentRay, 0.001, double.MaxValue, ref rec))
                {
                    if (rec.Material.Scatter(currentRay, ref rec, out Vec3 colorAttenuation, out Ray scattered))
                    {
                        color *= colorAttenuation;
                        currentRay = scattered;
                    }
                    else
                    {
                        return new Vec3(0.0, 0.0, 0.0);
                    }
                }
                else
                {
                    Vec3 unitDirection = currentRay.Direction.UnitVector();
                    double t = 0.5 * (unitDirection.y + 1.0);
                    color *= (1.0 - t) * vec3one + t * vec3numbers;
                    break;
                }
            }

            return color;
        }*/

        private Color RayColorToPixel(Vec3 color)
        {
            double R = color.x;
            double G = color.y;
            double B = color.z;

            // Divide the color by the number of samples and gamma-correct for gamma=2.0.
            double scale = 1.0 / SamplesPerPixel;
            R = Math.Sqrt(scale * R);
            G = Math.Sqrt(scale * G);
            B = Math.Sqrt(scale * B);

            return Color.FromArgb(
                (int)(256 * R.Clamp(0, .999)),
                (int)(256 * G.Clamp(0, .999)),
                (int)(256 * B.Clamp(0, .999))
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

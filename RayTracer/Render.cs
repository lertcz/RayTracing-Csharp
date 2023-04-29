﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;

namespace RayTracer
{
    internal class Render : INotifyPropertyChanged
    {
        readonly Random rnd = new Random(420);
        public event PropertyChangedEventHandler PropertyChanged;
        private double _renderProgress = 0;
        public double RenderProgress
        {
            get { return _renderProgress; }
            set
            {
                _renderProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RenderProgress"));
                Debug.WriteLine("Progress {0}", value);
            }
        }
        
        // Image
        const double aspect_ratio = 16.0 / 9.0;
        const int image_width = 400;
        const int image_height = (int)(image_width / aspect_ratio);
        const int SamplesPerPixel = 100;
        const int MaxDepth = 50;

        // World
        readonly HittableList World = new HittableList();
        public Render()
        {
            World.Add(new Sphere(new Vec3(0, 0, -1), 0.5));
            World.Add(new Sphere(new Vec3(0, -100.5, -1), 100));
        }

        // Camera
        readonly Camera Cam = new Camera();

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
                RenderProgress = (image_height - y) / image_height * 100;
            }

            return image;
        }

        private Vec3 RayColor(Ray r, Hittable World, int depth) {
            HitRecord rec = new HitRecord();

            // If we've exceeded the ray bounce limit, no more light is gathered.
            if (depth <= 0)
                return new Vec3(0.0, 0.0, 0.0);

            if (World.Hit(r, 0.001, double.MaxValue, ref rec))
            {
                Vec3 target = rec.P + rec.Normal + Vec3.RandomInUnitSphere();
                //Vec3 target = rec.P + rec.Normal + Vec3.RandomUnitVector();
                //Vec3 target = rec.P + Vec3.RandomInHemisphere(rec.Normal);
                return 0.5 * RayColor(new Ray(rec.P, target - rec.P), World, depth-1);
            }
            Vec3 unit_direction = r.Direction.UnitVector();
            double t = 0.5 * (unit_direction.y + 1.0);
            return (1.0-t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0);
        }

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

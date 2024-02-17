using System;
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
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RayTracer
{
    internal class Render : INotifyPropertyChanged
    {
        Random rnd; bool inProgress = false;
        public event PropertyChangedEventHandler PropertyChanged;
        private double _renderProgress = 0;
        public double RenderProgress
        {
            get { return _renderProgress; }
            set
            {
                _renderProgress = Math.Round(value, 0);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RenderProgress"));
                //Debug.WriteLine("Progress {0}", value);
            }
        }

        // Image Settings
        static float aspectRatio = 16.0f / 9.0f;
        static int image_width = 800; // 400
        static int image_height = (int)(image_width / aspectRatio);
        int SamplesPerPixel = 10; //100
        // 50 (min - 2 albedo, 3 metal, 5 glass) 
        static int MaxDepth = 10; // Maximum number of ray bounces into the scene

        // Camera Setup
        static Vec3 LookFrom, LookAt;
        double vfov = 40.0;
        readonly static Vec3 VectorUP = new Vec3(0, 1, 0);
        readonly static float distanceToFocus = 10f; //(LookFrom - LookAt).Length(); // auto focus
        static float aperture = 0.0f;
        Camera Cam; // = new Camera(LookFrom, LookAt, VectorUP, 20, aspectRatio, aperture, distanceToFocus, 0, 1);

        Vec3 Background = new Vec3(0, 0, 0);

        // Scene
        HittableList World;

        // Bitmap init
        public Bitmap Result;
        public System.Windows.Controls.Image Render_image { get; set; }

        public void Start(int scene)
        {
            if (inProgress) return; // failsafe

            switch (scene)
            {
                case 1:
                    World = Scenes.Part1RandomFinalScene();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    aperture = 0.1f;
                    break;

                case 2:
                    World = Scenes.MotionBlurScene();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    aperture = 0.1f; 
                    break;

                case 3:
                    World = Scenes.TwoSpheres();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;

                case 4:
                    World = Scenes.TwoPerlinSpheres();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;

                case 5:
                    World = Scenes.MarbleAndTurbulence();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;

                case 6:
                    World = Scenes.Earth();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    LookFrom = new Vec3(13, 2, 3);
                    LookAt = new Vec3(0, 0, 0);
                    vfov = 20;
                    break;

                case 7:
                    World = Scenes.SimpleLight();
                    SamplesPerPixel = 400;
                    LookFrom = new Vec3(26, 3, 6);
                    LookAt = new Vec3(0, 2, 0);
                    vfov = 20;
                    break;

                case 8:
                    World = Scenes.CornelBox();
                    aspectRatio = 1;
                    image_width = 600;
                    image_height = (int)(image_width / aspectRatio);
                    SamplesPerPixel = 500;
                    LookFrom = new Vec3(278, 278, -800);
                    LookAt = new Vec3(278, 278, 0);
                    vfov = 37;
                    break;

                case 9:
                default:
                    World = Scenes.LightShowcase();
                    Background = new Vec3(0.70, 0.80, 1.00);
                    aspectRatio = 1;
                    image_width = 300;
                    MaxDepth = 20;
                    image_height = (int)(image_width / aspectRatio);
                    SamplesPerPixel = 50;
                    LookFrom = new Vec3(278, 278, -800);
                    LookAt = new Vec3(278, 278, 0);
                    vfov = 37;
                    break;
            }
            Cam = new Camera(LookFrom, LookAt, VectorUP, vfov, aspectRatio, aperture, distanceToFocus, 0, 1);

            Debug.WriteLine("Render start");
            inProgress = true;
            new Thread(() =>
            {
                Bitmap image = new Bitmap(image_width, image_height);

                // Render
                // Bitmap img = SingleProcess();
                Bitmap img = MultiThread();

                Render_image.Dispatcher.Invoke(() => Render_image.Source = BitmapToImageSource(img));
                Result = img; // save Bitmap in a variable for saving into a file
                inProgress = false;

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
                        rnd = new Random();
                        double u = (x + rnd.NextDouble(0.0, 1.0)) / (image_width - 1);
                        double v = (y + rnd.NextDouble(0.0, 1.0)) / (image_height - 1);
                        Ray r = Cam.GetRay(u, v);
                        PixelColor += RayColor(r, Background, World, MaxDepth);
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


            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = Environment.ProcessorCount / 2;

            Parallel.For(0, image_height, y => // for (double y = 0; y < image_height; ++y)
            {

                var localPixels = new Tuple<int, int, Color>[image_width];
                for (int x = 0; x < image_width; ++x)
                {
                    Vec3 PixelColor = new Vec3(0, 0, 0);

                    for (int s = 0; s < SamplesPerPixel; ++s)
                    {
                        rnd = new Random();
                        double u = (x + rnd.NextDouble(0.0, 1.0)) / (image_width - 1);
                        double v = (y + rnd.NextDouble(0.0, 1.0)) / (image_height - 1);
                        Ray r = Cam.GetRay(u, v);
                        PixelColor += RayColor(r, Background, World, MaxDepth);
                    }
                    localPixels[x] = new Tuple<int, int, Color>(image_height - 1 - y, x, RayColorToPixel(PixelColor));
                }

                pixels.PushRange(localPixels);

                CompletedRows++;
                RenderProgress = Math.Round(CompletedRows / image_height * 100, 2);

            });

            Bitmap image = new Bitmap(image_width, image_height);

            while (!pixels.IsEmpty)
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

        //emitting  
        private Vec3 RayColor(Ray r, Vec3 background, Hittable World, int depth)
        {
            HitRecord rec = new HitRecord();

            // If we've exceeded the ray bounce limit, no more light is gathered.
            if (depth <= 0)
                return new Vec3(0, 0, 0);

            // If the ray hits nothing, return the background color.
            if (!World.Hit(r, 0.001, double.PositiveInfinity, ref rec))
                return background;

            Vec3 emitted = rec.Material.Emitted(rec.U, rec.V, rec.P);

            if (!rec.Material.Scatter(r, ref rec, out Vec3 colorAttenuation, out Ray scattered))
                return emitted;

            return emitted + colorAttenuation * RayColor(scattered, background, World, depth - 1);
        }

        // old
        //private Vec3 RayColor(Ray r, Hittable World, int depth)
        //{
        //    HitRecord rec = new HitRecord();

        //    // If we've exceeded the ray bounce limit, no more light is gathered.
        //    if (depth <= 0)
        //        return new Vec3(0.0, 0.0, 0.0);

        //    if (World.Hit(r, 0.001, double.MaxValue, ref rec))
        //    {
        //        if (rec.Material.Scatter(r, ref rec, out Vec3 colorAttenuation, out Ray scattered))
        //        {
        //            return colorAttenuation * RayColor(scattered, World, depth - 1);
        //        }
        //        return new Vec3(0.0, 0.0, 0.0);
        //    }
        //    Vec3 unit_direction = r.Direction.UnitVector();
        //    double t = 0.5 * (unit_direction.y + 1.0);
        //    return (1.0 - t) * new Vec3(1.0, 1.0, 1.0) + t * new Vec3(0.5, 0.7, 1.0);
        //}

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

            // Replace NaN components with zero.
            if (double.IsNaN(R)) R = 0.0;
            if (double.IsNaN(G)) G = 0.0;
            if (double.IsNaN(B)) B = 0.0;

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

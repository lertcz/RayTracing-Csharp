using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;

namespace RayTracer
{
    internal class Scenes
    {
        private readonly static Random random = new Random();
        public static HittableList Part1RandomFinalScene()
        {
            HittableList World = new HittableList();

            //Lambertian groundMaterial = new Lambertian(new Vec3(.5, .5, .5));
            //World.Add(new Sphere(new Vec3(0, -1000, 0), 1000, groundMaterial));

            Vec3 color1 = new Vec3(.2, .3, .1); Vec3 color2 = new Vec3(.9, .9, .9);
            CheckerTexture checker = new CheckerTexture(color1, color2);
            World.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(checker)));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMaterial = random.NextDouble();
                    Vec3 center = new Vec3(a + 0.9 * random.NextDouble(), .2, b + 0.9 * random.NextDouble());

                    if ((center - new Vec3(4, .2, 0)).Length() > .9)
                    {
                        if (chooseMaterial < .8)
                        {
                            // diffuse
                            Vec3 albedo = Vec3.Random() * Vec3.Random();
                            World.Add(new Sphere(center, .2, new Lambertian(albedo)));
                        }
                        else if (chooseMaterial < .95)
                        {
                            // metal
                            Vec3 albedo = Vec3.Random(0.5, 1);
                            double fuzz = random.NextDouble(0, .5);
                            World.Add(new Sphere(center, .2, new Metal(albedo, fuzz)));
                        }
                        else
                        {
                            // glass
                            World.Add(new Sphere(center, .2, new Dielectric(1.5)));
                        }
                    }
                }
            }

            World.Add(new Sphere(new Vec3(0, 1, 0), 1, new Dielectric(1.5)));
            World.Add(new Sphere(new Vec3(-4, 1, 0), 1, new Lambertian(new Vec3(.4, .2, .1))));
            World.Add(new Sphere(new Vec3(4, 1, 0), 1, new Metal(new Vec3(.7, .6, .5), 0)));

            return World;
        }

        public static HittableList MotionBlurScene()
        {
            HittableList World = new HittableList();

            Lambertian groundMaterial = new Lambertian(new Vec3(.5, .5, .5));
            World.Add(new Sphere(new Vec3(0, -1000, 0), 1000, groundMaterial));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMaterial = random.NextDouble();
                    Vec3 center = new Vec3(a + 0.9 * random.NextDouble(), .2, b + 0.9 * random.NextDouble());

                    if ((center - new Vec3(4, .2, 0)).Length() > .9)
                    {
                        if (chooseMaterial < .8)
                        {
                            // diffuse
                            Vec3 albedo = Vec3.Random() * Vec3.Random();
                            Vec3 center2 = center + new Vec3(0, random.NextDouble(0, .5), 0);
                            World.Add(new MovingSphere(center, center2, 0, 1, .2, new Lambertian(albedo)));
                        }
                        else if (chooseMaterial < .95)
                        {
                            // metal
                            Vec3 albedo = Vec3.Random(0.5, 1);
                            double fuzz = random.NextDouble(0, .5);
                            World.Add(new Sphere(center, .2, new Metal(albedo, fuzz)));
                        }
                        else
                        {
                            // glass
                            World.Add(new Sphere(center, .2, new Dielectric(1.5)));
                        }
                    }
                }
            }

            World.Add(new Sphere(new Vec3(0, 1, 0), 1, new Dielectric(1.5)));
            World.Add(new Sphere(new Vec3(-4, 1, 0), 1, new Lambertian(new Vec3(.4, .2, .1))));
            World.Add(new Sphere(new Vec3(4, 1, 0), 1, new Metal(new Vec3(.7, .6, .5), 0)));

            return World;
        }
    
        public static HittableList TwoSpheres()
        {
            HittableList Objects = new HittableList();

            Vec3 color1 = new Vec3(.2, .3, .1); Vec3 color2 = new Vec3(.9, .9, .9);
            Texture checker = new CheckerTexture(color1, color2);

            Objects.Add(new Sphere(new Vec3(0, -10, 0), 10, new Lambertian(checker)));
            Objects.Add(new Sphere(new Vec3(0, 10, 0), 10, new Lambertian(checker)));

            return Objects;
        }

        public static HittableList TwoPerlinSpheres()
        {
            HittableList Objects = new HittableList();

            Texture perlinTexture = new NoiseTexture(4);

            Objects.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));
            Objects.Add(new Sphere(new Vec3(0, 2, 0), 2, new Lambertian(perlinTexture)));

            return Objects;
        }

        public static HittableList MarbleAndTurbulence()
        {
            HittableList Objects = new HittableList();

            Texture turbulence = new TurbulentNoise(4);
            Texture marble = new MarbleTexture(4);

            Objects.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(turbulence)));
            Objects.Add(new Sphere(new Vec3(0, 2, 0), 2, new Lambertian(marble)));

            return Objects;
        }

        public static HittableList Earth()
        {
            HittableList Objects = new HittableList();

            Texture EarthTexture = new ImageTexture("earthmap.jpg");

            Objects.Add(new Sphere(new Vec3(0, 0, 0), 2, new Lambertian(EarthTexture)));

            return Objects;
        }
    
        public static HittableList SimpleLight()
        {
            HittableList Objects = new HittableList();

            Texture perlinTexture = new NoiseTexture(4);
            Objects.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));
            Objects.Add(new Sphere(new Vec3(0, 2, 0), 2, new Lambertian(perlinTexture)));

            DiffuseLight diffuseLight = new DiffuseLight(new Vec3(1, 1, 1));
            Objects.Add(new XYRect(3, 5, 1, 3, -2, diffuseLight));

            return Objects;
        }

        public static HittableList CornelBox()
        {
            HittableList Objects = new HittableList();

            Material Red = new Lambertian(new Vec3(.65, .05, .05));
            Material White = new Lambertian(new Vec3(.73, .73, .73));
            Material Green = new Lambertian(new Vec3(.12, .45, .15));
            //DiffuseLight Light = new DiffuseLight(new Vec3(15, 15, 15));
            DiffuseLight Light = new DiffuseLight(new Vec3(25, 25, 25));

            Objects.Add(new YZRect(0, 555, 0, 555, 555, Green));
            Objects.Add(new YZRect(0, 555, 0, 555, 0, Red));
            Objects.Add(new XZRect(213, 343, 227, 332, 554, Light));
            Objects.Add(new XZRect(0, 555, 0, 555, 0, White));
            Objects.Add(new XZRect(0, 555, 0, 555, 555, White));
            Objects.Add(new XYRect(0, 555, 0, 555, 555, White));

            Hittable box1 = new Box(new Vec3(0, 0, 0), new Vec3(165, 330, 165), White);
            box1 = new RotateY(box1, 15);
            box1 = new Translate(box1, new Vec3(265, 0, 295));
            Objects.Add(box1);

            Hittable box2 = new Box(new Vec3(0, 0, 0), new Vec3(165, 165, 165), White);
            box2 = new RotateY(box2, -18);
            box2 = new Translate(box2, new Vec3(130, 0, 65));
            Objects.Add(box2);

            return Objects;
        }

        public static HittableList LightShowcase()
        {
            HittableList Objects = new HittableList();

            Material Red = new Lambertian(new Vec3(.65, .05, .05));
            Material White = new Lambertian(new Vec3(.73, .73, .73));
            Material Green = new Lambertian(new Vec3(.12, .45, .15));
            Material Metal = new Metal(new Vec3(.7, .7, .7), .2);
            Material Mirror = new Metal(new Vec3(.85, .85, .85), 0);
            Material Glass = new Dielectric(1.5);
            Material Checker = new Lambertian(new CheckerTexture(new Vec3(.9, .9, .9), new Vec3(.1, .1, .1), .1));
            DiffuseLight Light = new DiffuseLight(new Vec3(1, 1, 1) * 30);

            //Objects.Add(new XZRect(213, 343, 227, 332, 554, Light));
            Objects.Add(new YZRect(0, 555, 0, 555, 555, Checker)); //left
            Objects.Add(new YZRect(0, 555, 0, 555, -.1, Checker)); // right
            Objects.Add(new XZRect(0, 555, 0, 555, .1, Checker)); // bottom
            Objects.Add(new XZRect(0, 555, 0, 555, 555, Checker)); // top
            Objects.Add(new XYRect(-.1, 555, 0, 555, 555, Checker)); // back

            //Objects.Add(new XYRect(0, 555, 0, 555, -700, Checker)); // front test
            //Hittable box1 = new Box(new Vec3(0, 0, 0), new Vec3(555, 555, 1), White);
            //box1 = new Translate(box1, new Vec3(0, 0, -800));
            //Objects.Add(box1);

            Objects.Add(new Sphere(new Vec3(278, 328, 0), 50, Metal));
            Objects.Add(new Sphere(new Vec3(278, 228, 0), 50, Red));
            Objects.Add(new Sphere(new Vec3(278, 128, 0), 50, Glass));

            //Hittable box1 = new Box(new Vec3(0, 0, 0), new Vec3(165, 330, 165), White);
            //box1 = new RotateY(box1, 15);
            //box1 = new Translate(box1, new Vec3(265, 0, 295));
            //Objects.Add(box1);

            //Hittable box2 = new Box(new Vec3(0, 0, 0), new Vec3(165, 165, 165), White);
            //box2 = new RotateY(box2, -18);
            //box2 = new Translate(box2, new Vec3(130, 0, 65));
            //Objects.Add(box2);

            return Objects;
        }
    }
}

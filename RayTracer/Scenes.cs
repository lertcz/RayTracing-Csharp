using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RayTracer
{
    internal class Scenes
    {
        private readonly static Random random = new Random();
        public static HittableList Part1RandomFinalScene()
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
    }
}

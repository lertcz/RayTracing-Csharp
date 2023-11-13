using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal abstract class Material
    {
        public abstract bool Scatter(Ray r, ref HitRecord rec, out Vec3 colorAttenuation, out Ray scattered);
    }

    class Lambertian : Material
    {
        Vec3 Albedo { get; set; }
        CheckerTexture AlbedoChecker { get; set; }

        public Lambertian(Vec3 texture)
        {
            Albedo = texture;
        }

        public Lambertian(CheckerTexture texture)
        {
            AlbedoChecker = texture;
        }

        public override bool Scatter(Ray r, ref HitRecord rec, out Vec3 colorAttenuation, out Ray scattered)
        {
            Vec3 scatterDirection = rec.Normal + Vec3.RandomUnitVector();

            // Catch degenerate scatter direction
            if (scatterDirection.NearZero())
                scatterDirection = rec.Normal;

            scattered = new Ray(rec.P, scatterDirection, r.Time);

            // look into it | not nice code

            if (AlbedoChecker != null)
            {
                colorAttenuation = AlbedoChecker.Value(rec.U, rec.V, rec.P);
            }
            else // solid color
            {
                colorAttenuation = Albedo;
            }

            return true;
        }
    }

    class Metal : Material
    {
        Vec3 Albedo { get; set; }
        double Fuzz { get; set; }

        public Metal(Vec3 color, double fuzz)
        {
            Albedo = color;
            Fuzz = fuzz;
        }

        public override bool Scatter(Ray r, ref HitRecord rec, out Vec3 colorAttenuation, out Ray scattered)
        {
            Vec3 reflected = Vec3.Reflect(r.Direction.UnitVector(), rec.Normal);
            scattered = new Ray(rec.P, reflected + Fuzz * Vec3.RandomInUnitSphere(), r.Time);
            colorAttenuation = Albedo;
            return scattered.Direction.Dot(rec.Normal) > 0;
        }
    }

    class Dielectric : Material
    {
        private readonly static Random random = new Random();
        double IR { get; set; } // Index of refraction

        public Dielectric(double indexOfRefraction)
        {
            IR = indexOfRefraction;
        }

        public override bool Scatter(Ray r, ref HitRecord rec, out Vec3 colorAttenuation, out Ray scattered)
        {
            colorAttenuation = new Vec3(1.0, 1.0, 1.0);
            double refractionRatio = rec.FrontFace ? (1.0 / IR) : IR;

            Vec3 unitDirection = r.Direction.UnitVector();
            double cosTheta = Math.Min(-unitDirection.Dot(rec.Normal), 1.0);
            double sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

            bool cannotRefract = refractionRatio * sinTheta > 1.0;
            Vec3 direction;

            if (cannotRefract || Reflectance(cosTheta, refractionRatio) > random.NextDouble())
                direction = Vec3.Reflect(unitDirection, rec.Normal);
            else
                direction = Vec3.Refract(unitDirection, rec.Normal, refractionRatio);

            scattered = new Ray(rec.P, direction, r.Time);
            return true;
        }

        private double Reflectance(double cosine, double refIdx)
        {
            // Use Schlick's approximation for reflectance.
            double r0 = (1 - refIdx) / (1 + refIdx);
            r0 *= r0;
            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
    }
}

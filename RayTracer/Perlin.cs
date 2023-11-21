using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Perlin
    {
        private readonly static Random random = new Random();

        private const int PointCount = 256;
        private readonly Vec3[] randomVector;
        private readonly int[] permX;
        private readonly int[] permY;
        private readonly int[] permZ;

        public Perlin()
        {
            randomVector = new Vec3[PointCount];
            for (int i = 0; i < PointCount; ++i)
            {
                randomVector[i] = Vec3.Random(-1, 1).UnitVector();
            }

            permX = PerlinGeneratePerm();
            permY = PerlinGeneratePerm();
            permZ = PerlinGeneratePerm();
        }

        public double Noise(Vec3 p)
        {
            double u = p.x - Math.Floor(p.x);
            double v = p.y - Math.Floor(p.y);
            double w = p.z - Math.Floor(p.z);

            int i = (int)Math.Floor(p.x);
            int j = (int)Math.Floor(p.y);
            int k = (int)Math.Floor(p.z);

            Vec3[,,] c = new Vec3[2, 2, 2];

            for (int di = 0; di < 2; di++)
                for (int dj = 0; dj < 2; dj++)
                    for (int dk = 0; dk < 2; dk++)
                        c[di, dj, dk] = randomVector[
                            permX[(i + di) & 255] ^
                            permY[(j + dj) & 255] ^
                            permZ[(k + dk) & 255]
                        ];

            return PerlinInterp(c, u, v, w);
        }

        public double Turb(Vec3 p, int depth=7)
        {
            double accum = 0;
            Vec3 tempP= p;
            double weight = 1;

            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5;
                tempP *= 2;
            }

            return Math.Abs(accum);
        }

        private static double PerlinInterp(Vec3[,,] c, double u, double v, double w)
        {
            double uu = u * u * (3 - 2 * u);
            double vv = v * v * (3 - 2 * v);
            double ww = w * w * (3 - 2 * w);
            double accum = 0.0;

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        Vec3 weightV = new Vec3(u - i, v - j, w - k);
                        accum += (i * uu + (1 - i) * (1 - uu)) *
                                 (j * vv + (1 - j) * (1 - vv)) *
                                 (k * ww + (1 - k) * (1 - ww)) *
                                 c[i, j, k].Dot(weightV);
                    }

            return accum;
        }

        private static int[] PerlinGeneratePerm()
        {
            int[] p = new int[PointCount];

            for (int i = 0; i < PointCount; i++)
            {
                p[i] = i;
            }

            Permute(p, PointCount);

            return p;
        }

        private static void Permute(int[] p, int n)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int target = random.Next(0, i);
                (p[target], p[i]) = (p[i], p[target]); // swap
            }
        }
    }
}

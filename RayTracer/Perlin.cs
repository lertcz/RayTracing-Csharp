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
        private readonly double[] ranFloat;
        private readonly int[] permX;
        private readonly int[] permY;
        private readonly int[] permZ;

        public Perlin()
        {
            ranFloat = new double[PointCount];
            for (int i = 0; i < PointCount; ++i)
            {
                ranFloat[i] = random.NextDouble();
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

            // Hermite cubic to round off the interpolation:
            u = u * u * (3 - 2 * u);
            v = v * v * (3 - 2 * v);
            w = w * w * (3 - 2 * w);

            int i = (int)Math.Floor(p.x);
            int j = (int)Math.Floor(p.y);
            int k = (int)Math.Floor(p.z);

            double[,,] c = new double[2, 2, 2];

            for (int di = 0; di < 2; di++)
                for (int dj = 0; dj < 2; dj++)
                    for (int dk = 0; dk < 2; dk++)
                        c[di, dj, dk] = ranFloat[
                            permX[(i + di) & 255] ^
                            permY[(j + dj) & 255] ^
                            permZ[(k + dk) & 255]
                        ];

            return TrilinearInterpolation(c, u, v, w);
        }

        private static double TrilinearInterpolation(double[,,] c, double u, double v, double w)
        {
            double accum = 0.0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        accum += (i * u + (1 - i) * (1 - u)) *
                                 (j * v + (1 - j) * (1 - v)) *
                                 (k * w + (1 - k) * (1 - w)) * c[i, j, k];
                    }
                }
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

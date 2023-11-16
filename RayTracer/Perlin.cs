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
            int i = (int)(4 * p.x) & 255;
            int j = (int)(4 * p.y) & 255;
            int k = (int)(4 * p.z) & 255;

            return ranFloat[permX[i] ^ permY[j] ^ permZ[k]];
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }

    public static class DoubleExtensions
    {
        public static double Clamp(
            this double value,
            double min,
            double max)
        {
            return Math.Min(max, Math.Max(value, min));
        }
    }
}

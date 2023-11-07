using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    internal class Interval
    {
        public double Min { get; }
        public double Max { get; }

        public static readonly Interval Empty = new Interval(double.PositiveInfinity, double.NegativeInfinity);
        public static readonly Interval Universe = new Interval(double.NegativeInfinity, double.PositiveInfinity);

        public Interval() : this(double.PositiveInfinity, double.NegativeInfinity)
        {
        }

        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(double x)
        {
            return Min <= x && x <= Max;
        }

        public bool Surrounds(double x)
        {
            return Min < x && x < Max;
        }
    }
}

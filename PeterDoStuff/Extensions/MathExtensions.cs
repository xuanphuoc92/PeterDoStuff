﻿namespace PeterDoStuff.Extensions
{
    public static class MathExtensions
    {
        public static double Cap(this double value, double min, double max)
        {
            double range = max - min;
            while (value >= max)
                value -= range;
            while (value < min)
                value += range;
            return value;
        }
    }
}
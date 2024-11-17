namespace PeterDoStuff.Extensions
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

        public static double RadToDeg(this double rad) => rad / Math.PI * 180;

        public static double DegToRad(this double deg) => deg / 180 * Math.PI;
    }
}

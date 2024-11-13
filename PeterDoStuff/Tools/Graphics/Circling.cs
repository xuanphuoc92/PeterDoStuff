
namespace PeterDoStuff.Tools.Graphics
{
    public class Circling : Animation
    {
        public Circling(double centerX, double centerY, double radius)
        {
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
        }

        public double CenterX, CenterY, Radius, Phase;
        public TimeSpan CirclingPeriod = TimeSpan.FromSeconds(1);

        public override async Task Resolve(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double deltaPhase = timeSpan.TotalNanoseconds / CirclingPeriod.TotalNanoseconds;
            Phase += deltaPhase;

            if (Phase > 1)
                Phase -= (int)Phase;

            double radian = Phase * 2 * Math.PI;
            Model.X = CenterX + Radius * Math.Sin(radian);
            Model.Y = CenterY - Radius * Math.Cos(radian);
        }
    }
}

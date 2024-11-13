
namespace PeterDoStuff.Tools.Graphics
{
    public class Rotating : Animation
    {
        public TimeSpan RotatingPeriod = TimeSpan.FromSeconds(1);
        public double Phase;

        protected override async Task Resolve(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double deltaPhase = timeSpan.TotalNanoseconds / RotatingPeriod.TotalNanoseconds;
            Phase += deltaPhase;

            if (Phase > 1)
                Phase -= (int)Phase;

            Model.Degree = Phase * 360;
        }
    }
}

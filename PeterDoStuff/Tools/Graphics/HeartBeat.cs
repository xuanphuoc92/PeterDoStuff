namespace PeterDoStuff.Tools.Graphics
{
    public class HeartBeat : Animation
    {
        public double MinScale, MaxScale;
        public TimeSpan BeatPeriod = TimeSpan.FromSeconds(1);
        public double GrowPhase = 0.2;
        public double Phase = 0;

        public HeartBeat(double minScale, double maxScale)
        {
            (MinScale, MaxScale) = (minScale, maxScale);
        }

        public override async Task Resolve(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double deltaPhase = timeSpan.TotalNanoseconds / BeatPeriod.TotalNanoseconds;
            Phase += deltaPhase;

            if (Phase > 1)
                Phase -= (int)Phase;

            Model.Scale = Phase < GrowPhase
                ? (MinScale + Phase / GrowPhase * (MaxScale - MinScale))
                : (MaxScale - (Phase - GrowPhase) / (1 - GrowPhase) * (MaxScale - MinScale));
        }
    }
}

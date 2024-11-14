namespace PeterDoStuff.Tools.Graphics
{
    public class HeartBeat(double minScale, double maxScale, double startPhase = 0)
        : Effect
    {
        public double
            MinScale = minScale,
            MaxScale = maxScale;
        public TimeSpan BeatPeriod = TimeSpan.FromSeconds(1);
        public double GrowPhase = 0.2;
        public double Phase = startPhase;

        protected override async Task Resolve(DateTime now)
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

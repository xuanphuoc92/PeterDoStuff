namespace PeterDoStuff.Tools.Graphics
{
    public class Blink : Animation
    {
        public TimeSpan BlinkGap = TimeSpan.FromSeconds(1);
        public double Phase = 0;
        public double MinX, MaxX, MinY, MaxY;
        private double RangeX => MaxX - MinX;
        private double RangeY => MaxY - MinY;
        
        private Random Random = new Random();

        protected override async Task Resolve(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double deltaPhase = timeSpan.TotalNanoseconds / BlinkGap.TotalNanoseconds;
            Phase += deltaPhase;

            if (Phase > 1)
            {
                Phase -= (int)Phase;
                Model.X = MinX + Random.NextDouble() * RangeX;
                Model.Y = MinY + Random.NextDouble() * RangeY;
            }
        }
    }
}

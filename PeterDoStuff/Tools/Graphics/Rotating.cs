namespace PeterDoStuff.Tools.Graphics
{
    public class Rotating(double startPhase = 0) : Effect
    {
        public TimeSpan Period = TimeSpan.FromSeconds(1);
        public double Phase = startPhase;

        protected override async Task Resolve(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double deltaPhase = timeSpan.TotalNanoseconds / Period.TotalNanoseconds;
            Phase += deltaPhase;

            if (Phase > 1)
                Phase -= (int)Phase;

            Model.Degrees = Phase * 360;
        }
    }
}

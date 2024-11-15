namespace PeterDoStuff.Tools.Graphics
{
    public class Clockwising(double centerX, double centerY, double radius, double startPhase = 0)
        : Effect
    {
        public double
            CenterX = centerX,
            CenterY = centerY,
            Radius = radius,
            Phase = startPhase;

        public TimeSpan Period = TimeSpan.FromSeconds(1);

        protected override async Task Tick(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double deltaPhase = timeSpan.TotalNanoseconds / Period.TotalNanoseconds;
            Phase += deltaPhase;

            if (Phase > 1)
                Phase -= (int)Phase;

            double radian = Phase * 2 * Math.PI;
            Model.X = CenterX + Radius * Math.Sin(radian);
            Model.Y = CenterY - Radius * Math.Cos(radian);
        }
    }
}

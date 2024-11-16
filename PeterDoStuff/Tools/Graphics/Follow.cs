namespace PeterDoStuff.Tools.Graphics
{
    public class Follow(Model anchor)
        : Effect
    {
        public Model Anchor = anchor;

        public double Velocity = 100;
        public double SlowRange = 1;
        public double StopRange = 1;
        public double MergeRange = 1;

        protected override async Task Tick(DateTime now)
        {
            (double d, double dx, double dy) = Model.GetDistance(Anchor);

            if (d > StopRange)
            {
                var timeSpan = FromLastTick(now);
                var delta = timeSpan.TotalSeconds * Math.Min(Velocity, d / SlowRange * Velocity);

                var deltaX = delta / d * dx;
                var deltaY = delta / d * dy;

                Model.Move(deltaX, deltaY);
                Model.PointTo(deltaX, deltaY);
            }            

            if (d <= MergeRange)
            {
                Model.MoveTo(Anchor);
            }
        }
    }
}

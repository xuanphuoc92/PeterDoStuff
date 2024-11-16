namespace PeterDoStuff.Tools.Graphics
{
    public class DistanceConstraint(Model anchor, double maxDistance, double minDistance = 0) : Effect
    {
        public Model Anchor = anchor;
        public double MaxDistance = maxDistance;
        public double MinDistance = minDistance;

        protected override Task Tick(DateTime now)
        {
            (double d, double dx, double dy) = Model.GetDistance(Anchor);

            if (d < MinDistance && d > 0)
            {
                Model.X = Anchor.X - MinDistance * dx / d;
                Model.Y = Anchor.Y - MinDistance * dy / d;
            }

            if (d > MaxDistance)
            {
                Model.X = Anchor.X - MaxDistance * dx / d;
                Model.Y = Anchor.Y - MaxDistance * dy / d;
            }

            return Task.CompletedTask;
        }
    }
}

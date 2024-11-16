namespace PeterDoStuff.Tools.Graphics
{
    public class DistanceConstraint(Model anchor, double distance) : Effect
    {
        public Model Anchor = anchor;
        public double Distance = distance;
        public double PointingRange = 1;

        protected override Task Tick(DateTime now)
        {
            (double d, double dx, double dy) = Model.GetDistance(Anchor);

            if (d > Distance)
            {
                Model.X = Anchor.X - Distance * dx / d;
                Model.Y = Anchor.Y - Distance * dy / d;
            }

            if (d > PointingRange)
                Model.PointTo(dx, dy);

            return Task.CompletedTask;
        }
    }
}

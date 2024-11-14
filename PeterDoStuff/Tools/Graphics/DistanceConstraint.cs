namespace PeterDoStuff.Tools.Graphics
{
    public class DistanceConstraint(Model anchor, double distance) : Effect
    {
        public Model Anchor = anchor;
        public double Distance = distance;
        public double PointingRange = 1;

        protected override Task Resolve(DateTime now)
        {
            double dx = Anchor.X - Model.X;
            double dy = Anchor.Y - Model.Y;
            double d = Math.Sqrt(dx * dx + dy * dy);

            if (d > Distance)
            {
                Model.X = Anchor.X - Distance * dx / d;
                Model.Y = Anchor.Y - Distance * dy / d;
            }

            if (d > PointingRange)
                Model.Degrees = PointTo(dx, dy);

            return Task.CompletedTask;
        }
    }
}

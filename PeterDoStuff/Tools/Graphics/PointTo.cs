
namespace PeterDoStuff.Tools.Graphics
{
    public class PointTo(Model anchor, double pointingRange = 1) : Effect
    {
        public Model Anchor = anchor;
        public double PointingRange = pointingRange;

        protected override Task Tick(DateTime now)
        {
            (double d, double dx, double dy) = Model.GetDistance(Anchor);
            Model.PointTo(dx, dy);
            return Task.CompletedTask;
        }
    }
}

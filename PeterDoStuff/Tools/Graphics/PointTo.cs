
namespace PeterDoStuff.Tools.Graphics
{
    public class PointTo(Model anchor) : Effect
    {
        public Model Anchor = anchor;

        protected override Task Tick(DateTime now)
        {
            Model.PointTo(Anchor);
            return Task.CompletedTask;
        }
    }
}

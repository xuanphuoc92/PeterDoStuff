
namespace PeterDoStuff.Tools.Graphics
{
    public class MoveTo(Model anchor) : Effect
    {
        public Model Anchor = anchor;

        protected override Task Tick(DateTime now)
        {
            Model.MoveTo(anchor);
            return Task.CompletedTask;
        }
    }
}

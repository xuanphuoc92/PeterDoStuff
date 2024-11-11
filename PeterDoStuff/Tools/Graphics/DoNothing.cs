namespace PeterDoStuff.Tools.Graphics
{
    public class DoNothing : Animation
    {
        public override Task Tick(Model model, DateTime? now = null) => Task.CompletedTask;
    }
}

namespace PeterDoStuff.Tools.Graphics
{
    public class DoNothing : Animation
    {
        public override Task Tick(Model model, TimeSpan? timeSpan = null) => Task.CompletedTask;
    }
}

namespace PeterDoStuff.Tools.Graphics
{
    public class DoNothing : Animation
    {
        public Task Tick(Model model, DateTime? now = null) => Task.CompletedTask;
    }
}

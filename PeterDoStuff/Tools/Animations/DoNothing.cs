namespace PeterDoStuff.Tools.Animation
{
    public class DoNothing : Animation
    {
        public Task Tick(Model model) => Task.CompletedTask;
    }
}

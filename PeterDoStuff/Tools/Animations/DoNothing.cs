namespace PeterDoStuff.Tools.Animations
{
    public class DoNothing : Animation
    {
        public Task Tick(Model model) => Task.CompletedTask;
    }
}

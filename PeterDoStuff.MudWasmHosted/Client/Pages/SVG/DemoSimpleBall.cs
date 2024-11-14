using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoSimpleBall : Canvas
    {
        public DemoSimpleBall(Style style) : base(300, 300, style)
        {
            Name = "Simple Ball";

            var circle = new Circle(25) { X = Width / 2, Y = Height / 2 };
            circle.AddEffect(new BouncingBall(0, Width, 0, Height, 250, 0));
            AddModel(circle);
        }
    }
}

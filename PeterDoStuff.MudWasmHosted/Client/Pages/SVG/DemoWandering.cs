using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoWandering : Canvas
    {
        public DemoWandering(Style style) : base(300, 300, style)
        {
            Name = "Wandering";
            
            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            var wander = new Wander(30, Width - 30, 30, Height - 30);
            wander.Follow.Velocity = 250;
            wander.Follow.SlowRange = 200;
            circle.AddEffect(wander);

            AddModel(circle);
        }
    }
}

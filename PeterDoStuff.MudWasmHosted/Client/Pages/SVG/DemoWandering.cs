using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoWandering : Canvas
    {
        public DemoWandering(ModelStyler? builder) : base(300, 300, builder)
        {
            Name = "Wandering";
            
            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            circle.AddAnimation(new Wander(30, Width - 30, 30, Height - 30)
            {
                Velocity = 250,
                SlowRange = 200
            });

            AddAndStyleModel(circle);
        }
    }
}

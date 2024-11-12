using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoWandering : Canvas
    {
        public DemoWandering(ModelStyler? builder) : base(300, 300, builder)
        {
            Name = "Wandering";
            
            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            circle.AddAnimation(new Wander()
            {
                MinX = 30, MaxX = Width - 30,
                MinY = 30, MaxY = Height - 30,
                Velocity = 250,
                SlowRange = 200
            });

            AddAndStyleModel(circle);
        }
    }
}

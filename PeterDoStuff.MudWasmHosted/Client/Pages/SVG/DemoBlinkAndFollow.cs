using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBlinkAndFollow : Canvas
    {
        public DemoBlinkAndFollow(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Blink and Follow";

            var anchor = new Circle(10) { X = Width / 2, Y = Height / 2 };
            anchor.AddAnimation(new Blink() {
                MinX = 30,
                MaxX = Width - 30,
                MinY = 30,
                MaxY = Height - 30,
            });

            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            circle.AddAnimation(new Follow()
            {
                Anchor = anchor,
                Velocity = 250,
                SlowRange = 200
            });

            AddAndStyleModel(anchor);
            AddAndStyleModel(circle);
        }
    }
}

using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBlinkAndFollow : Canvas
    {
        public DemoBlinkAndFollow() : base(300, 300)
        {
            Name = "Blink and Follow";

            var anchor = new Circle(10) { X = Width / 2, Y = Height / 2 };
            anchor.StrokeWidth = 10;
            anchor.AddAnimation(new Blink() {
                MinX = 30,
                MaxX = Width - 30,
                MinY = 30,
                MaxY = Height - 30,
            });

            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            circle.StrokeWidth = 10;
            circle.AddAnimation(new Follow()
            {
                Anchor = anchor,
                Velocity = 100
            });

            AddModel(anchor);
            AddModel(circle);
        }
    }
}

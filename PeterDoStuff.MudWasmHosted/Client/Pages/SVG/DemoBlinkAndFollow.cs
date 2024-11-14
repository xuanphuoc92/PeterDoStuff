using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBlinkAndFollow : Canvas
    {
        public DemoBlinkAndFollow(Style style) : base(300, 300, style)
        {
            Name = "Blink and Follow";

            var anchor = new Circle(10) { X = Width / 2, Y = Height / 2 };
            anchor.AddEffect(new Blink(30, Width - 30, 30, Height - 30));

            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            circle.AddEffect(new Follow(anchor)
            {
                Velocity = 500,
                SlowRange = 100,
                StopRange = 2,
                MergeRange = 2,
            });

            AddModel(anchor);
            AddModel(circle);
        }
    }
}

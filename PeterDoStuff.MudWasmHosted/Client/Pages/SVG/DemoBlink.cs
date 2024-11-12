using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoBlink : Canvas
    {
        public DemoBlink() : base(300, 300)
        {
            Name = "Blink";

            var circle = new Circle(10) { X = Width / 2, Y = Height / 2 };
            circle.StrokeWidth = 10;
            circle.AddAnimation(new Blink() {
                MinX = 30,
                MaxX = Width - 30,
                MinY = 30,
                MaxY = Height - 30,
            });

            AddModel(circle);
        }
    }
}

using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoHeartBeat : Canvas
    {
        public DemoHeartBeat() : base(300, 300)
        {
            Name = "Heart Beat";

            CanvasRect.StrokeWidth = 4;

            var circle = new Circle(50) { X = Width / 2, Y = Height / 2};
            circle.StrokeWidth = 20;
            circle.AddAnimation(new HeartBeat(1, 1.5));

            AddModel(circle);
        }
    }
}

using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoHeartBeat : Canvas
    {
        public DemoHeartBeat(Style style) : base(300, 300, style)
        {
            Name = "Heart Beat";

            var circle = new Circle(50) { X = Width / 2, Y = Height / 2};
            circle.AddEffect(new HeartBeat(1, 1.5));
            AddModel(circle);
        }
    }
}

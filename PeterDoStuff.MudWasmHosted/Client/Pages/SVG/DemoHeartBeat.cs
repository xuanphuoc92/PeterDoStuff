using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoHeartBeat : Canvas
    {
        public DemoHeartBeat(ModelBuilder builder) : base(300, 300, builder)
        {
            Name = "Heart Beat";

            var circle = new Circle(50) { X = Width / 2, Y = Height / 2};
            circle.StrokeWidth = 20;
            circle.AddAnimation(new HeartBeat(1, 1.5));

            AddModel(circle);
        }
    }
}

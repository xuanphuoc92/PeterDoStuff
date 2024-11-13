using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoArrowPlane : Canvas
    {
        public DemoArrowPlane(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Arrow Plane";

            var now = DateTime.Now;

            double size = 20;

            var polygon = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
            polygon.X = Width / 2;
            polygon.Y = Height / 2;
            
            polygon.AddAnimation(new HeartBeat(0.75, 2) { LastTick = now, BeatPeriod = TimeSpan.FromMilliseconds(500) });
            polygon.AddAnimation(new Rotating() { LastTick = now, Phase = 0.25 });
            polygon.AddAnimation(new Circling(Width / 2, Height / 2, 100) { LastTick = now });

            AddAndStyleModel(polygon);
        }
    }
}

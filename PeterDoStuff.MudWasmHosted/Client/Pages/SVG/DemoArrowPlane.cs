using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoArrowPlane : Canvas
    {
        public DemoArrowPlane(Style style) : base(300, 300, style)
        {
            Name = "Arrow Plane";

            var now = DateTime.Now;

            double size = 20;

            var polygon = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
            polygon.X = Width / 2;
            polygon.Y = Height / 2;
            polygon.Degrees = 90;
            
            polygon.AddEffect(new HeartBeat(0, 1) { LastTick = now, BeatPeriod = TimeSpan.FromSeconds(5), GrowPhase = 0.5 });
            polygon.AddEffect(new Rotating(360) { LastTick = now });
            polygon.AddEffect(new Clockwising(Width / 2, Height / 2, 100) { LastTick = now });

            AddAndStyle(polygon);
        }
    }
}

using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoTCurve : Canvas
    {
        public DemoTCurve(Style style) : base(300, 300, style)
        {
            Name = "T Quad Curve";

            var path = new PathModel(50, 150);
            path.QuadCurveTo(100, 150);
            path.QuadCurveTo(150, 150);
            path.QuadCurveTo(200, 150);
            path.QuadCurveTo(250, 150);

            AddAndStyle(path);

            path.Style.SetFill("", 0);

            DateTime now = DateTime.Now;
            path.Models[1].AddEffect(new BouncingInBox(0, Width, 50, Height - 50, 0, -250) { LastTick = now });
            path.Models[3].AddEffect(new BouncingInBox(0, Width, 50, Height - 50, 0, 250) { LastTick = now });
        }
    }
}

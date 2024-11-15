using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoCurve : Canvas
    {
        public DemoCurve(Style style) : base(300, 300, style)
        {
            Name = "Curve";

            var path = new PathModel(50, 150);
            path.QuadCurveTo(100, 150, 150, 150);
            path.QuadCurveTo(200, 150, 250, 150);

            AddAndStyle(path);

            path.Style.SetFill("", 0);

            DateTime now = DateTime.Now;
            path.Commands[1].Points[0].AddEffect(new BouncingInBox(0, Width, 50, Height - 50, 0, -100) { LastTick = now });
            path.Commands[2].Points[0].AddEffect(new BouncingInBox(0, Width, 50, Height - 50, 0, 100) { LastTick = now });
        }
    }
}

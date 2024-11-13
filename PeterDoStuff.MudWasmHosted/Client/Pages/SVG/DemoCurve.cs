using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoCurve : Canvas
    {
        public DemoCurve(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Curve";

            var path = new PathModel(50, 150);
            path.SmoothCurveTo(150, 150, 100, 150);
            path.SmoothCurveTo(250, 150, 200, 150);

            AddAndStyleModel(path);

            path.FillOpacity = 0;

            DateTime now = DateTime.Now;
            path.Commands[1].Params[0].AddAnimation(new BouncingInBox(0, Width, 50, Height - 50, 0, -50) { LastTick = now });
            path.Commands[2].Params[0].AddAnimation(new BouncingInBox(0, Width, 50, Height - 50, 0, 50) { LastTick = now });
        }
    }
}

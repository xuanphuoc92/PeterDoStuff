using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoPath : Canvas
    {
        public DemoPath(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Path";

            var path = new PathModel(50, 150);
            path.LineTo(100, 150);
            path.LineTo(150, 150);
            path.LineTo(200, 150);
            path.LineTo(250, 150);

            AddAndStyleModel(path);

            path.FillOpacity = 0;

            DateTime now = DateTime.Now;
            path.Children[1].AddAnimation(new BouncingInBox(0, Width, 50, Height - 50, 0, -250) { LastTick = now });
            path.Children[3].AddAnimation(new BouncingInBox(0, Width, 50, Height - 50, 0, 250) { LastTick = now });
        }
    }
}

using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoPath : Canvas
    {
        public DemoPath(Style style) : base(300, 300, style)
        {
            Name = "Path";

            var path = new PathModel(50, 150);
            path.LineTo(100, 150);
            path.LineTo(150, 150);
            path.LineTo(200, 150);
            path.LineTo(250, 150);

            AddModel(path);

            path.Style = Style.Clone().SetFill("", 0);

            DateTime now = DateTime.Now;
            path.Children[1].AddEffect(new BouncingInBox(0, Width, 50, Height - 50, 0, -250) { LastTick = now });
            path.Children[3].AddEffect(new BouncingInBox(0, Width, 50, Height - 50, 0, 250) { LastTick = now });
        }
    }
}

using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoPath : Canvas
    {
        public DemoPath(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Path";

            var path = new PathModel(50, 150);
            path.LineTo(100, 100);
            path.LineTo(150, 150);
            path.LineTo(200, 200);
            path.LineTo(250, 150);

            AddAndStyleModel(path);

            path.FillOpacity = 0;
        }
    }
}

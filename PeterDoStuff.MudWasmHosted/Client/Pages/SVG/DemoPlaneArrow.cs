using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoPlaneArrow : Canvas
    {
        public DemoPlaneArrow(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Plane Arrow";

            double size = 20;

            var polygon = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
            polygon.X = Width / 2;
            polygon.Y = Height / 2;

            AddAndStyleModel(polygon);
        }
    }
}

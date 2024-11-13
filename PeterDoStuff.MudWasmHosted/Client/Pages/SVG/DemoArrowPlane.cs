using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoArrowPlane : Canvas
    {
        public DemoArrowPlane(ModelStyler builder) : base(300, 300, builder)
        {
            Name = "Arrow Plane";

            double size = 20;

            var polygon = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
            polygon.X = Width / 2;
            polygon.Y = Height / 2;

            var rotating = new Rotating();
            polygon.AddAnimation(rotating);

            AddAndStyleModel(polygon);
        }
    }
}

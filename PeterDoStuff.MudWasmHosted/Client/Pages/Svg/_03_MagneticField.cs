using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _03_MagneticField : CanvasModel
    {
        public _03_MagneticField(Style? style = null) : base(300, 300, style, new CircleModel(10))
        {
            Name = "Magnetic Field";

            Mouse.Style.StrokeOpacity *= 0.5;
            Mouse.Style.FillOpacity *= 0.5;

            for (int x = 50; x <= 250; x += 50)
            {
                for (int y = 50; y <= 250; y += 50)
                {
                    var polygon = CreatePolygon(x, y);
                    polygon.Apply(new RotateTo(Mouse));
                    AddAndStyle(polygon);
                }
            }
        }

        private PolygonModel CreatePolygon(double x, double y)
        {
            double size = 10;
            var polygon = new PolygonModel(size * 2, 0, size * -1, size, 0, 0, size * -1, size * -1);
            polygon.X = x;
            polygon.Y = y;
            return polygon;
        }
    }
}

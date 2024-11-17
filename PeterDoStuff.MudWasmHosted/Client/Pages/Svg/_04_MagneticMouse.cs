using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;
using PeterDoStuff.Graphics.Effects;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _04_MagneticMouse : CanvasModel
    {
        public _04_MagneticMouse(Style? style = null) : base(300, 300, style, new CircleModel(10))
        {
            Name = "Magnetic Mouse";

            Mouse.Style.StrokeOpacity *= 0.5;
            Mouse.Style.FillOpacity *= 0.5;

            var anchor = new CircleModel(50);
            anchor.X = 150;
            anchor.Y = 150;
            AddAndStyle(anchor);

            double size = 15;
            var polygon = new PolygonModel(size * 2, 0, size * -1, size, 0, 0, size * -1, size * -1);
            polygon.X = 150;
            polygon.Y = 150;
            polygon.Apply(new RotateTo(Mouse) { Range = 5 });
            polygon.Apply(new MoveTo(Mouse));
            polygon.Apply(new DistanceConstraint(anchor, 50));
            AddAndStyle(polygon);

            var decor = new CircleModel(2);
            var moveTo = new MoveTo(polygon);
            moveTo.Offset.X = 40;
            decor.Apply(moveTo);
            AddAndStyle(decor);
        }
    }
}

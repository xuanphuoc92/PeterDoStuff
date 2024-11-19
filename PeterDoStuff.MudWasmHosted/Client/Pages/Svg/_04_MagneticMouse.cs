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

            var arrow = new Arrow(15, 150, 150);
            arrow.Apply(new RotateTo(Mouse) { Range = 5 });
            arrow.Apply(new MoveTo(Mouse));
            arrow.Apply(new DistanceConstraint(anchor, 50));
            AddAndStyle(arrow);

            var decor = new CircleModel(2);
            var moveTo = new MoveTo(arrow);
            moveTo.Offset.X = 40;
            decor.Apply(moveTo);
            AddAndStyle(decor);
        }
    }
}

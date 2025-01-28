using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;
using PeterDoStuff.Graphics.Effects;
using MudBlazor;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _04_MagneticMouse : CanvasModel
    {
        public _04_MagneticMouse(int width, int height, Style? style = null) : base(width, height, style)
        {
            Name = "Magnetic Mouse";

            var pointer = new CircleModel(10);
            AddAndStyle(pointer);
            pointer.Style.StrokeOpacity *= 0.5;
            pointer.Style.FillOpacity *= 0.5;
            pointer.Apply(new Follow(Mouse, 250));

            var anchor = new CircleModel(50);
            anchor.X = width / 2;
            anchor.Y = height / 2;
            AddAndStyle(anchor);

            var arrow = new Arrow(15, 150, 150);
            arrow.Apply(new PointTo(pointer) { Range = 5 });
            arrow.Apply(new StickTo(pointer));
            arrow.Apply(new DistanceConstraint(anchor, 50));
            AddAndStyle(arrow);

            var decor = new CircleModel(2);
            var stickTo = new StickTo(arrow);
            stickTo.Offset.X = 40;
            decor.Apply(stickTo);
            AddAndStyle(decor);
        }
    }
}

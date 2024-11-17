using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _02_DistanceConstraint : CanvasModel
    {
        public _02_DistanceConstraint(Style? style = null) : base(300, 300, style, new CircleModel(50))
        {
            Name = "Distance Constraint";

            Mouse.Style.StrokeOpacity *= 0.5;
            Mouse.Style.FillOpacity *= 0.1;

            var circle = new CircleModel(10);
            circle.Apply(new DistanceConstraint(Mouse, 50));
            AddAndStyle(circle);
        }
    }
}

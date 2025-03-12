using MudBlazor;
using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _02_DistanceConstraint : CanvasModel
    {
        public _02_DistanceConstraint(int width, int height, Style? style = null) : base(width, height, style)
        {
            Name = "Distance Constraint";

            var pointer = new CircleModel(50);
            AddAndStyle(pointer);
            pointer.Style.StrokeOpacity *= 0.5;
            pointer.Style.FillOpacity *= 0.1;
            pointer.Apply(new Follow(Mouse, 250));

            var circle = new CircleModel(10);
            circle.Apply(new DistanceConstraint(pointer, 50));
            AddAndStyle(circle);
        }
    }
}

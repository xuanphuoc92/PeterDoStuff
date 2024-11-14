using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoDistanceConstraint : Canvas
    {
        public DemoDistanceConstraint(Style? style = null) : base(300, 300, style)
        {
            Name = "Distance Constraint";

            Mouse = new Circle(50);
            Mouse.Style = Style.Clone();
            Mouse.Style.StrokeOpacity = 0.5;
            Mouse.Style.StrokeDashArray = "5%";

            var circle = new Circle(10);
            circle.AddEffect(new DistanceConstraint(Mouse, 50));

            AddAndStyle(circle);
        }
    }
}

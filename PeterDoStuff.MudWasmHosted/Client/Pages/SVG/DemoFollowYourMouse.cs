using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoFollowYourMouse : Canvas
    {
        public DemoFollowYourMouse(Style style) : base(300, 300, style)
        {
            Name = "Follow Your Mouse";

            Mouse = new Circle(10);
            Mouse.Style = Style.Clone();
            Mouse.Style.StrokeOpacity = 0.2;

            double size = 20;

            var arrowHead = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
            arrowHead.X = Width / 2;
            arrowHead.Y = Height / 2;
            arrowHead.AddEffect(new Follow(Mouse)
            {
                Velocity = 250,
                SlowRange = 50,
                StopRange = 2,
                MergeRange = 2,
            });
            Mouse.X = arrowHead.X; Mouse.Y = arrowHead.Y;

            AddAndStyle(arrowHead);

            Model anchor = arrowHead;
            var distance = 20;

            for (int i = 0; i < 10; i++)
            {
                var bodyJoint = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
                bodyJoint.Scale = 0.5;
                bodyJoint.Degrees = 90;
                bodyJoint.X = arrowHead.X; bodyJoint.Y = arrowHead.Y;
                bodyJoint.AddEffect(new PointTo(anchor));
                bodyJoint.AddEffect(new DistanceConstraint(anchor, distance, distance));
                AddAndStyle(bodyJoint);
                anchor = bodyJoint;
            }
        }
    }
}

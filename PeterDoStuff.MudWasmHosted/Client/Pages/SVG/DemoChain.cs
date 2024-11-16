using PeterDoStuff.Tools.Graphics;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.SVG
{
    public class DemoChain : Canvas
    {
        public DemoChain(Style style) : base(300, 300, style)
        {
            Name = "Chain";

            var now = DateTime.Now;

            var startX = Width / 2;
            var startY = Height / 2 - 100;

            double size = 20;

            var arrowHead = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
            arrowHead.X = startX;
            arrowHead.Y = startY;
            arrowHead.Degrees = 90;

            var period = TimeSpan.FromSeconds(2);

            arrowHead.AddEffect(new Rotating(period) 
            {
                LastTick = now,
            });
            arrowHead.AddEffect(new Clockwising(Width / 2, Height / 2, 100) 
            { 
                Period = period,
                LastTick = now,
            });

            AddAndStyle(arrowHead);

            Model anchor = arrowHead;
            var distance = 20;

            for (int i = 0; i < 15; i++)
            {
                var bodyJoint = new Polygon(0, size * -2, size, size, 0, 0, size * -1, size);
                bodyJoint.Scale = 0.5;
                bodyJoint.Degrees = 90;
                bodyJoint.X = startX;
                bodyJoint.Y = startY;
                bodyJoint.AddEffect(new PointTo(anchor));
                bodyJoint.AddEffect(new DistanceConstraint(anchor, distance));
                AddAndStyle(bodyJoint);
                anchor = bodyJoint;
            }
        }
    }
}

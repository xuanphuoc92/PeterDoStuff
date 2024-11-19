using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class Gallery(Style style)
    {
        public CanvasModel[] Canvases = [
            new _01_BouncingBalls(style, new CircleModel(10)),
            new _02_DistanceConstraint(style),
            new _03_MagneticField(style),
            new _04_MagneticMouse(style),
            new _05_MouseFollower(style),
            new _06_Chains(style),
            new _07_AngleConstraint(style),
        ];
    }

    public class DistanceConstraint(Model anchor, double maxDistance) : Effect
    {
        public Model Anchor = anchor;
        public double MaxDistance = maxDistance;
        
        public override void Tick()
        {
            Models.ForEach(m =>
            {
                var dx = Anchor.X - m.X;
                var dy = Anchor.Y - m.Y;
                var d = Math.Sqrt(dx * dx + dy * dy);

                if (d > MaxDistance)
                {
                    m.X = Anchor.X - MaxDistance * dx / d;
                    m.Y = Anchor.Y - MaxDistance * dy / d;
                }
            });
        }
    }
}

using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class Gallery(Style style)
    {
        public List<CanvasModel> Canvases = [
            new _01_BouncingBalls(style, new CircleModel(10)),
            new _02_DistanceConstraint(style),
            new _03_MagneticField(style),
            new _04_MagneticMouse(style),
            new _05_MouseFollower(style),
            new _06_Chains(style),
            new _07_AngleConstraint(style),
            new _08_Snake(style),
        ];
    }

    public class DistanceConstraint(Model anchor, double maxDistance) : Effect
    {
        public Model Anchor = anchor;
        public double MaxDistance = maxDistance;
        
        public override void Tick()
        {
            var dx = Anchor.X - Model.X;
            var dy = Anchor.Y - Model.Y;
            var d = Math.Sqrt(dx * dx + dy * dy);

            if (d > MaxDistance)
            {
                Model.X = Anchor.X - MaxDistance * dx / d;
                Model.Y = Anchor.Y - MaxDistance * dy / d;
            }
        }
    }
}

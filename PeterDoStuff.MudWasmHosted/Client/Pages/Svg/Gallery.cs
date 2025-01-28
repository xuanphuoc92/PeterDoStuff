using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class Gallery
    {
        public List<CanvasModel> Canvases = [];

        public Gallery(Style style)
        {
            Canvases = CanvasFactories
                .Select(fact => fact(style))
                .ToList();
        }

        public List<Func<Style, CanvasModel>> CanvasFactories = [
            style => new _01_BouncingBalls(style, new CircleModel(10)),
            style => new _02_DistanceConstraint(style),
            style => new _03_MagneticField(style),
            style => new _04_MagneticMouse(style),
            style => new _05_MouseFollower(style),
            style => new _06_Chains(style),
            style => new _07_AngleConstraint(style),
            style => new _08_Snake(style),
            style => new _09_Fish(style),
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

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
                .Select(fact => fact(300, 300, style))
                .ToList();
        }

        public List<Func<int, int, Style, CanvasModel>> CanvasFactories = [
            (width, height, style) => new _01_BouncingBalls(width, height, style, new CircleModel(10)),
            (width, height, style) => new _02_DistanceConstraint(width, height, style),
            (width, height, style) => new _03_MagneticField(width, height, style),
            (width, height, style) => new _04_MagneticMouse(width, height, style),
            (width, height, style) => new _05_MouseFollower(width, height, style),
            (width, height, style) => new _06_Chains(width, height, style),
            (width, height, style) => new _07_AngleConstraint(width, height, style),
            (width, height, style) => new _08_Snake(width, height, style),
            (width, height, style) => new _09_Fish(width, height, style),
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

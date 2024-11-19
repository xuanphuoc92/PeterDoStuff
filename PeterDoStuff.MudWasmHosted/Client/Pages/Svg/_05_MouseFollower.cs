using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;
using PeterDoStuff.Graphics.Models;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _05_MouseFollower : CanvasModel
    {
        public _05_MouseFollower(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Mouse Follower";

            double size = 15;
            var polygon = new PolygonModel(size * 2, 0, size * -1, size, 0, 0, size * -1, size * -1);
            polygon.X = 150;
            polygon.Y = 150;

            var follow = new Follow(Mouse, 250);
            follow.SlowRange = 100;
            polygon.Apply(follow);

            AddAndStyle(polygon);
        }
    }

    public class Follow(Model anchor, double v) : Effect
    {
        public Model Anchor = anchor;
        public double V = v;
        public double SlowRange = 1;
        public double StopRange = 1;
        public double MergeRange = 1;

        public override void Tick()
        {
            Models.ForEach(model =>
            {
                var dx = Anchor.X - model.X;
                var dy = Anchor.Y - model.Y;
                var d = Math.Sqrt(dx * dx + dy * dy);

                if (d > StopRange)
                {
                    var delta = TimeFromLastTick.TotalSeconds * Math.Min(V, d / SlowRange * V);
                    var deltaX = delta / d * dx;
                    var deltaY = delta / d * dy;

                    model.X += deltaX;
                    model.Y += deltaY;

                    var rotateTo = new RotateTo(anchor);                    
                    rotateTo.Resolve(model);
                }

                if (d <= MergeRange)
                {
                    var moveTo = new MoveTo(anchor);
                    moveTo.Resolve(model);
                }    
            });
        }
    }
}

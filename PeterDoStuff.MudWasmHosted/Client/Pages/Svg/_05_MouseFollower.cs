using PeterDoStuff.Graphics;
using PeterDoStuff.Graphics.Effects;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.Svg
{
    public class _05_MouseFollower : CanvasModel
    {
        public _05_MouseFollower(Style? style = null, Model? mouse = null) : base(300, 300, style, mouse)
        {
            Name = "Mouse Follower";

            var arrow = new Arrow(15, 150, 150);

            var follow = new Follow(Mouse, 250);
            follow.SlowRange = 100;
            arrow.Apply(follow);

            AddAndStyle(arrow);
        }
    }

    public class Follow(Model anchor, double v) : Effect
    {
        public Model Anchor = anchor;
        public double V = v;
        public double SlowRange = v / 5;
        public double StopRange = 5;
        public double MergeRange = 2;

        public override void Tick()
        {
            var dx = Anchor.X - Model.X;
            var dy = Anchor.Y - Model.Y;
            var d = Math.Sqrt(dx * dx + dy * dy);

            if (d > StopRange)
            {
                var delta = TimeFromLastTick.TotalSeconds * Math.Min(V, d / SlowRange * V);
                var deltaX = delta / d * dx;
                var deltaY = delta / d * dy;

                Model.X += deltaX;
                Model.Y += deltaY;

                var pointTo = new PointTo(Anchor);
                pointTo.Resolve(Model);
            }

            if (d <= MergeRange)
            {
                var stickTo = new StickTo(Anchor);
                stickTo.Resolve(Model);
            }
        }
    }
}

namespace PeterDoStuff.Graphics.Effects
{
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

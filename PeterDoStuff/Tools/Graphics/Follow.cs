namespace PeterDoStuff.Tools.Graphics
{
    public class Follow(Model anchor)
        : Effect
    {
        public Model Anchor = anchor;

        public double Velocity = 100;
        public double SlowRange = 1;
        public double StopRange = 1;
        public double MergeRange = 1;

        protected override async Task Tick(DateTime now)
        {
            var dx = Anchor.X - Model.X;
            var dy = Anchor.Y - Model.Y;
            var d = Math.Sqrt(dx * dx + dy * dy); // pythagoras theorem, duh

            if (d <= StopRange)
            {
                if (d <= MergeRange)
                {
                    Model.X = Anchor.X;
                    Model.Y = Anchor.Y;
                }
                return;
            }

            var timeSpan = FromLastTick(now);            
            var delta = timeSpan.TotalSeconds * Math.Min(Velocity, d / SlowRange * Velocity);

            var deltaX = delta / d * dx;
            var deltaY = delta / d * dy;

            Model.X += deltaX;
            Model.Y += deltaY;

            Model.PointTo(dx, dy);
        }
    }
}

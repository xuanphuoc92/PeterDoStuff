namespace PeterDoStuff.Tools.Graphics
{
    public class Follow : Animation
    {
        public Vector Anchor;

        public Follow(Vector anchor)
        {
            Anchor = anchor;
        }

        public double Velocity = 100;
        public double SlowRange = 0;
        public double StopRange = 0;
        public double MergeRange = 0;

        protected override async Task Resolve(DateTime now)
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
        }
    }
}


namespace PeterDoStuff.Tools.Graphics
{
    public class Follow : Animation
    {
        public Vector Anchor;
        public double Velocity;

        public override async Task Tick(TimeSpan? timeSpan = null)
        {
            var dx = Anchor.X - Model.X;
            var dy = Anchor.Y - Model.Y;
            var d = Math.Sqrt(dx * dx + dy * dy);

            if (d == 0)
                return;

            double time = timeSpan.Value.TotalSeconds;
            var delta = time * Velocity;

            var deltaX = delta / d * dx;
            var deltaY = delta / d * dy;

            Model.X += deltaX;
            Model.Y += deltaY;
        }
    }
}

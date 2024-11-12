
namespace PeterDoStuff.Tools.Graphics
{
    public class Pull : Animation
    {
        public Vector Anchor;
        public double Velocity = 100;

        public override async Task Tick(TimeSpan? timeSpan = null)
        {
            double time = (timeSpan == null ? 0 : timeSpan.Value.TotalSeconds);
            var delta = time * Velocity;

            var dx = Anchor.X - Model.X;
            var dy = Anchor.Y - Model.Y;
            var d = Math.Sqrt(dx * dx + dy * dy);

            if (d == 0)
                return;

            var deltaX = delta / d * dx;
            var deltaY = delta / d * dy;

            Model.X += deltaX;
            Model.Y += deltaY;
        }
    }
}

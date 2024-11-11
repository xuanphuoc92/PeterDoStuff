
namespace PeterDoStuff.Tools.Graphics
{
    public class BouncingInBox : Animation
    {
        public BouncingInBox(double Left, double Right, double Top, double Bottom, double VelocityX, double VelocityY)
            => (this.Left, this.Right, this.Top, this.Bottom, this.VelocityX, this.VelocityY) = (Left, Right, Top, Bottom, VelocityX, VelocityY);

        public double Left, Right, Top, Bottom, VelocityX, VelocityY;

        public override async Task Tick(Model model, TimeSpan? timeSpan = null)
        {
            double time = (timeSpan == null ? 1 : timeSpan.Value.TotalSeconds);

            var deltaX = time * VelocityX;
            var deltaY = time * VelocityY;

            model.X += deltaX;
            model.Y += deltaY;

            if (model.X < Left || model.X > Right)
                VelocityX *= -1;

            if (model.Y < Top || model.Y > Bottom)
                VelocityY *= -1;

            if (model.X < Left)
                model.X = Left;

            if (model.X > Right)
                model.X = Right;

            if (model.Y < Top)
                model.Y = Top;

            if (model.Y > Bottom)
                model.Y = Bottom;
        }
    }
}


namespace PeterDoStuff.Tools.Graphics
{
    public class BouncingInBox : Animation
    {
        public BouncingInBox(double Left, double Right, double Top, double Bottom, double VelocityX, double VelocityY)
            => (this.Left, this.Right, this.Top, this.Bottom, this.VelocityX, this.VelocityY) = (Left, Right, Top, Bottom, VelocityX, VelocityY);

        public double Left, Right, Top, Bottom, VelocityX, VelocityY;

        protected override async Task Resolve(DateTime now)
        {
            var timeSpan = FromLastTick(now);
            double time = timeSpan.TotalSeconds;

            var deltaX = time * VelocityX;
            var deltaY = time * VelocityY;

            Model.X += deltaX;
            Model.Y += deltaY;

            if (Model.X < Left || Model.X > Right)
                VelocityX *= -1;

            if (Model.Y < Top || Model.Y > Bottom)
                VelocityY *= -1;

            if (Model.X < Left)
                Model.X = Left;

            if (Model.X > Right)
                Model.X = Right;

            if (Model.Y < Top)
                Model.Y = Top;

            if (Model.Y > Bottom)
                Model.Y = Bottom;
        }
    }
}

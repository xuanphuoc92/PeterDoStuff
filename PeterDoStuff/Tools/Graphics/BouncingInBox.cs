namespace PeterDoStuff.Tools.Graphics
{
    public class BouncingInBox(double left, double right, double top, double bottom, double vx, double vy)
        : Effect
    {
        public double
            Left = left,
            Right = right,
            Top = top,
            Bottom = bottom,
            VelocityX = vx,
            VelocityY = vy;

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

namespace PeterDoStuff.Tools.Graphics
{
    public class BouncingBall(double left, double right, double top, double bottom, double vx, double vy)
        : Effect
    {
        public double 
            Left = left,
            Right = right,
            Top = top, 
            Bottom = bottom,
            VelocityX = vx,
            VelocityY = vy;

        private BouncingInBox _bouncingInBox = new(left, right, top, bottom, vx, vy);

        protected override Task Resolve(DateTime now)
        {
            var circle = (Circle)Model;
            double ballRadius = (circle.Radius + (circle.Style.StrokeWidth / 2)) * circle.Scale;
            _bouncingInBox.Model = Model;
            _bouncingInBox.Left = Left + ballRadius;
            _bouncingInBox.Right = Right - ballRadius;
            _bouncingInBox.Top = Top + ballRadius;
            _bouncingInBox.Bottom = Bottom - ballRadius;

            return _bouncingInBox.Tick(now);
        }
    }
}
